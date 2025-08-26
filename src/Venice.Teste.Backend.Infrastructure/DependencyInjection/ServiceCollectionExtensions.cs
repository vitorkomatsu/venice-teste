using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Infrastructure.DbContexts;
using Venice.Teste.Backend.Infrastructure.Repositories;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using Venice.Teste.Backend.Domain.Entities;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using MassTransit;
using Venice.Teste.Backend.Domain.Services;
using Venice.Teste.Backend.Infrastructure.Cache;
using Venice.Teste.Backend.Domain.Messaging;
using Venice.Teste.Backend.Infrastructure.Messaging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Venice.Teste.Backend.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceContexts(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IApplicationDbContext,ApplicationDbContext>();
            AddDbContext(services, configuration);
            AddMongo(services, configuration);
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderItemRepository, OrderItemRepository>();
        }

        public static void AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConn = configuration.GetConnectionString("Redis") ?? configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConn;
                options.InstanceName = "venice:";
            });
            services.AddSingleton<ICacheService, RedisCacheService>();
        }

        public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitHost = configuration.GetValue<string>("RabbitMq:Host") ?? "rabbitmq";
            var rabbitUser = configuration.GetValue<string>("RabbitMq:Username") ?? "guest";
            var rabbitPass = configuration.GetValue<string>("RabbitMq:Password") ?? "guest";

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitHost, "/", h =>
                    {
                        h.Username(rabbitUser);
                        h.Password(rabbitPass);
                    });
                });
            });

            services.AddScoped<IPedidoCriadoPublisher, MassTransitPedidoCriadoPublisher>();
        }

        [ExcludeFromCodeCoverage]
        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(
                     options => options.UseLazyLoadingProxies()
                     .UseInMemoryDatabase("ApplicationConnection")
                 );
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(
                     options => options.UseLazyLoadingProxies()
                     .UseSqlServer(configuration.GetConnectionString("ApplicationConnection"),
                     b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                 );
            }
        }

        private static void AddMongo(IServiceCollection services, IConfiguration configuration)
        {
            // Ensure GUIDs are always serialized/deserialized using the Standard (RFC 4122) representation
            // This avoids runtime errors like: GuidSerializer cannot serialize a Guid when GuidRepresentation is Unspecified
            try
            {
                // Register a global Guid serializer with Standard representation
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

                // Also register a convention to apply Standard representation across mapped types
                var pack = new ConventionPack { new GuidRepresentationConvention(GuidRepresentation.Standard) };
                ConventionRegistry.Register("GuidConventions", pack, _ => true);
            }
            catch
            {
                // Swallow if already registered to avoid throwing during app startup in warm reloads/tests
            }

            var mongoConn = configuration.GetConnectionString("MongoConnection") ?? "mongodb://localhost:27017";
            var mongoDb = configuration.GetSection("Mongo")["Database"] ?? "venice_orders";

            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(mongoDb);
            });
            services.AddSingleton<IMongoCollection<OrderItem>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                var collection = database.GetCollection<OrderItem>("order_items");
                var indexKeys = Builders<OrderItem>.IndexKeys.Ascending(i => i.OrderId);
                collection.Indexes.CreateOne(new CreateIndexModel<OrderItem>(indexKeys));
                return collection;
            });
        }
    }
}
