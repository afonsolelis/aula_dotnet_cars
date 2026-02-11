using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Volkswagen.Dashboard.Repository.Documents;

namespace Volkswagen.Dashboard.Repository
{
    public class MongoSchemaInitializer : IMongoSchemaInitializer
    {
        private readonly IMongoDatabase _database;

        public MongoSchemaInitializer(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var collectionNames = await (await _database.ListCollectionNamesAsync(cancellationToken: cancellationToken))
                .ToListAsync(cancellationToken);

            await EnsureCollectionAsync("cars", BuildCarsSchema(), collectionNames, cancellationToken);
            await EnsureCollectionAsync("users", BuildUsersSchema(), collectionNames, cancellationToken);
            await EnsureCollectionAsync("email_whitelist", BuildWhitelistSchema(), collectionNames, cancellationToken);

            await EnsureIndexesAsync(cancellationToken);
            await SeedAsync(cancellationToken);
        }

        private async Task EnsureCollectionAsync(string name, BsonDocument schema, List<string> existingCollections, CancellationToken cancellationToken)
        {
            if (existingCollections.Contains(name))
            {
                return;
            }

            var options = new CreateCollectionOptions<BsonDocument>
            {
                Validator = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument("$jsonSchema", schema)),
                ValidationAction = DocumentValidationAction.Error
            };

            await _database.CreateCollectionAsync(name, options, cancellationToken);
        }

        private async Task EnsureIndexesAsync(CancellationToken cancellationToken)
        {
            var users = _database.GetCollection<UserDocument>("users");
            var whitelist = _database.GetCollection<EmailWhitelistDocument>("email_whitelist");

            var userEmailIndex = new CreateIndexModel<UserDocument>(
                Builders<UserDocument>.IndexKeys.Ascending(x => x.Credentials.Email),
                new CreateIndexOptions { Unique = true, Name = "ux_users_credentials_email" });

            var whitelistEmailIndex = new CreateIndexModel<EmailWhitelistDocument>(
                Builders<EmailWhitelistDocument>.IndexKeys.Ascending(x => x.Email),
                new CreateIndexOptions { Unique = true, Name = "ux_whitelist_email" });

            await users.Indexes.CreateOneAsync(userEmailIndex, cancellationToken: cancellationToken);
            await whitelist.Indexes.CreateOneAsync(whitelistEmailIndex, cancellationToken: cancellationToken);
        }

        private async Task SeedAsync(CancellationToken cancellationToken)
        {
            var cars = _database.GetCollection<CarModel>("cars");
            var users = _database.GetCollection<UserDocument>("users");
            var whitelist = _database.GetCollection<EmailWhitelistDocument>("email_whitelist");

            var defaultEmails = new[]
            {
                "admin@vw.com",
                "teste@vw.com",
                "aluno@inteli.edu.br",
                "professor@inteli.edu.br",
                "dev@volkswagen.com.br"
            };

            foreach (var email in defaultEmails)
            {
                var exists = await whitelist.Find(x => x.Email == email).AnyAsync(cancellationToken);
                if (!exists)
                {
                    await whitelist.InsertOneAsync(new EmailWhitelistDocument
                    {
                        Email = email,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken: cancellationToken);
                }
            }

            var hasCars = await cars.Find(FilterDefinition<CarModel>.Empty).AnyAsync(cancellationToken);
            if (!hasCars)
            {
                var seedCars = new List<CarModel>
                {
                    new() { Name = "Gol", DateRelease = new DateTime(1980, 5, 15, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "Polo", DateRelease = new DateTime(2002, 3, 10, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "Golf", DateRelease = new DateTime(1974, 5, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "Jetta", DateRelease = new DateTime(1979, 6, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "Tiguan", DateRelease = new DateTime(2007, 9, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "T-Cross", DateRelease = new DateTime(2019, 4, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "Nivus", DateRelease = new DateTime(2020, 6, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "Amarok", DateRelease = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "Virtus", DateRelease = new DateTime(2018, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new() { Name = "Saveiro", DateRelease = new DateTime(1982, 8, 1, 0, 0, 0, DateTimeKind.Utc) }
                };

                await cars.InsertManyAsync(seedCars, cancellationToken: cancellationToken);
            }

            var hasAdmin = await users.Find(x => x.Credentials.Email == "admin@vw.com").AnyAsync(cancellationToken);
            if (!hasAdmin)
            {
                var whitelistEntry = await whitelist.Find(x => x.Email == "admin@vw.com").FirstOrDefaultAsync(cancellationToken);

                await users.InsertOneAsync(new UserDocument
                {
                    Profile = new UserProfile { Username = "Administrador" },
                    Credentials = new UserCredentials
                    {
                        Email = "admin@vw.com",
                        PasswordHash = GetMd5Hash("admin123")
                    },
                    WhitelistEntryId = whitelistEntry?.Id,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken: cancellationToken);
            }
        }

        private static BsonDocument BuildCarsSchema()
        {
            return new BsonDocument
            {
                { "bsonType", "object" },
                { "required", new BsonArray { "_id", "name", "dateRelease" } },
                {
                    "properties", new BsonDocument
                    {
                        { "_id", new BsonDocument("bsonType", "objectId") },
                        { "name", new BsonDocument("bsonType", "string") },
                        { "dateRelease", new BsonDocument("bsonType", "date") }
                    }
                }
            };
        }

        private static BsonDocument BuildUsersSchema()
        {
            return new BsonDocument
            {
                { "bsonType", "object" },
                { "required", new BsonArray { "_id", "profile", "credentials", "createdAt" } },
                {
                    "properties", new BsonDocument
                    {
                        { "_id", new BsonDocument("bsonType", "objectId") },
                        {
                            "profile", new BsonDocument
                            {
                                { "bsonType", "object" },
                                { "required", new BsonArray { "username" } },
                                {
                                    "properties", new BsonDocument
                                    {
                                        { "username", new BsonDocument("bsonType", "string") }
                                    }
                                }
                            }
                        },
                        {
                            "credentials", new BsonDocument
                            {
                                { "bsonType", "object" },
                                { "required", new BsonArray { "email", "passwordHash" } },
                                {
                                    "properties", new BsonDocument
                                    {
                                        { "email", new BsonDocument("bsonType", "string") },
                                        { "passwordHash", new BsonDocument("bsonType", "string") }
                                    }
                                }
                            }
                        },
                        { "whitelistEntryId", new BsonDocument("bsonType", "objectId") },
                        { "createdAt", new BsonDocument("bsonType", "date") }
                    }
                }
            };
        }

        private static BsonDocument BuildWhitelistSchema()
        {
            return new BsonDocument
            {
                { "bsonType", "object" },
                { "required", new BsonArray { "_id", "email", "createdAt" } },
                {
                    "properties", new BsonDocument
                    {
                        { "_id", new BsonDocument("bsonType", "objectId") },
                        { "email", new BsonDocument("bsonType", "string") },
                        { "createdAt", new BsonDocument("bsonType", "date") }
                    }
                }
            };
        }

        private static string GetMd5Hash(string input)
        {
            using var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();

            foreach (var value in data)
            {
                builder.Append(value.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
