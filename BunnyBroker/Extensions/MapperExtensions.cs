using BunnyBroker.Entities;

namespace BunnyBroker.Extensions;

internal static class MapperExtensions {
	public static Contracts.BunnyMessageItem MapToContract(this BunnyMessage bunny) {
		return new Contracts.BunnyMessageItem {
			Id = bunny.Id,
			CreatedAt = bunny.CreatedAt,
			BunnyType = bunny.BunnyType,
		};
	}

	public static Contracts.BunnyMessageInfo MapToContractInfo(this BunnyMessage bunny)
	{
		return new Contracts.BunnyMessageInfo
		{
			Id = bunny.Id,
			Body = bunny.Body,
			CreatedAt = bunny.CreatedAt,
			BunnyType = bunny.BunnyType,
			Logs = bunny.BunnyLogs.Map(x=>x.MapToInfo())
        };
	}

    public static BunnyMessage MapToEntity(this Contracts.BunnyMessage bunny) {
		return new BunnyMessage {
			Id = bunny.Id,
			Body = bunny.Body,
			CreatedAt = bunny.CreatedAt,
			BunnyType = bunny.BunnyType
		};
	}

	public static Contracts.BunnyTypeRegistryItem MapToContract(this BunnyTypeRegistry registry) {
		return new Contracts.BunnyTypeRegistryItem {
			BunnyType = registry.BunnyType,
			BunnyHandlerType = registry.BunnyHandlerType,
		};
    }

	public static Contracts.BunnyLogItem MapToContract(this BunnyLog log) {
		return new Contracts.BunnyLogItem
        {
			BunnyMessageId = log.BunnyMessageId,
            BunnyHandlerType = log.BunnyHandlerType,
			ProcessedAt = log.ProcessedAt,
			Sucess = log.Sucess,
			ErrorMessage = log.ErrorMessage
        };
    }

	private static Contracts.LogInfo MapToInfo(this BunnyLog log)
	{
		return new Contracts.LogInfo
        {
			BunnyHandlerType = log.BunnyHandlerType,
			ProcessedAt = log.ProcessedAt,
			Sucess = log.Sucess,
			ErrorMessage = log.ErrorMessage
		};
	}

    private static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> source, Func<T,TResult> mapper)
	{
		return source?.Select(item => mapper(item)) ?? Enumerable.Empty<TResult>();
    }
}
