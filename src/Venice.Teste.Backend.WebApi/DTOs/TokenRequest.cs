namespace Venice.Teste.Backend.WebApi.DTOs;

public record TokenRequest(string? Sub, string? Name, int? ExpMinutes);