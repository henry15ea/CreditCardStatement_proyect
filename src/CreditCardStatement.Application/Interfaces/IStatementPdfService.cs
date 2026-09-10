using CreditCardStatement.Application.DTOs;

namespace CreditCardStatement.Application.Interfaces
{
    public interface IStatementPdfService
    {
        byte[] GenerateStatementPdf(StatementDto statement, int month, int year);
    }
}
