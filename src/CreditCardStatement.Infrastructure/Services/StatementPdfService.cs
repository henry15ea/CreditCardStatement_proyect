using CreditCardStatement.Application.DTOs;
using CreditCardStatement.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace CreditCardStatement.Infrastructure.Services
{
    public class StatementPdfService : IStatementPdfService
    {
        public byte[] GenerateStatementPdf(StatementDto statement, int month, int year)
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Segoe UI"));

                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Mi Banco Online").Bold().FontSize(20).FontColor("#0d1b2a");
                                col.Item().Text("Estado de Cuenta").FontSize(14).FontColor("#415a77");
                            });

                            row.ConstantItem(120).Column(col =>
                            {
                                col.Item().AlignRight().Text($"{monthName} {year}").FontSize(12).FontColor("#778da9");
                                col.Item().AlignRight().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).FontColor("#778da9");
                            });
                        });

                        header.Item().PaddingVertical(10).LineHorizontal(1).LineColor("#e0e0e0");
                    });

                    page.Content().Column(content =>
                    {
                        // Titular y tarjeta
                        content.Item().Background("#f4f6f8").Padding(15).Column(cardInfo =>
                        {
                            cardInfo.Item().Text("Informacion de la Tarjeta").Bold().FontSize(12).FontColor("#0d1b2a");
                            cardInfo.Item().PaddingTop(5).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Titular").FontSize(9).FontColor("#778da9");
                                    col.Item().Text(statement.CreditCard.CardHolderName).Bold().FontSize(12);
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Numero de Tarjeta").FontSize(9).FontColor("#778da9");
                                    var lastFour = statement.CreditCard.CardNumber.Length >= 4 ? statement.CreditCard.CardNumber[^4..] : statement.CreditCard.CardNumber;
                                    col.Item().Text($"****{lastFour}").Bold().FontSize(12);
                                });
                            });
                        });

                        content.Item().PaddingVertical(15);

                        // Resumen financiero
                        content.Item().Text("Resumen Financiero").Bold().FontSize(14).FontColor("#0d1b2a");
                        content.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text("Saldo Actual").FontColor("#778da9").FontSize(10);
                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text(statement.CreditCard.CurrentBalance.ToString("C")).Bold();

                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text("Limite de Credito").FontColor("#778da9").FontSize(10);
                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text(statement.CreditCard.CreditLimit.ToString("C"));

                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text("Saldo Disponible").FontColor("#778da9").FontSize(10);
                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text(statement.AvailableBalance.ToString("C")).FontColor("#2ecc71").Bold();

                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text("Compras del Mes").FontColor("#778da9").FontSize(10);
                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text(statement.TotalPurchasesCurrentMonth.ToString("C"));

                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text("Compras Mes Anterior").FontColor("#778da9").FontSize(10);
                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text(statement.TotalPurchasesPreviousMonth.ToString("C"));

                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text("Interes Bonificable").FontColor("#778da9").FontSize(10);
                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text(statement.BonifiableInterest.ToString("C")).FontColor("#e74c3c");

                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text("Cuota Minima").FontColor("#778da9").FontSize(10);
                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text(statement.MinimumPayment.ToString("C")).Bold();

                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text("Total a Pagar").FontColor("#778da9").FontSize(10);
                            table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text(statement.TotalToPay.ToString("C")).Bold().FontSize(12).FontColor("#0d1b2a");

                            table.Cell().Padding(8).Text("Contado con Intereses").FontColor("#778da9").FontSize(10);
                            table.Cell().Padding(8).AlignRight().Text(statement.CashPaymentWithInterest.ToString("C")).Bold().FontColor("#e74c3c");
                        });

                        content.Item().PaddingVertical(15);

                        // Transacciones
                        content.Item().Text("Transacciones del Mes").Bold().FontSize(14).FontColor("#0d1b2a");
                        content.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#0d1b2a").Padding(8).Text("Fecha").FontColor(Colors.White).Bold().FontSize(10);
                                header.Cell().Background("#0d1b2a").Padding(8).Text("Descripcion").FontColor(Colors.White).Bold().FontSize(10);
                                header.Cell().Background("#0d1b2a").Padding(8).Text("Tipo").FontColor(Colors.White).Bold().FontSize(10);
                                header.Cell().Background("#0d1b2a").Padding(8).AlignRight().Text("Monto").FontColor(Colors.White).Bold().FontSize(10);
                            });

                            foreach (var t in statement.Transactions)
                            {
                                var typeLabel = t.TypeName == "Purchase" ? "Compra" : "Pago";
                                var typeColor = t.TypeName == "Purchase" ? "#e74c3c" : "#2ecc71";
                                var amountPrefix = t.TypeName == "Purchase" ? "-" : "+";

                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text(t.Date.ToString("dd/MM/yyyy")).FontSize(10);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text(t.Description).FontSize(10);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).Text(typeLabel).FontColor(typeColor).Bold().FontSize(10);
                                table.Cell().BorderBottom(1).BorderColor("#e0e0e0").Padding(8).AlignRight().Text($"{amountPrefix}{t.Amount.ToString("C")}").FontColor(typeColor).FontSize(10);
                            }

                            if (!statement.Transactions.Any())
                            {
                                table.Cell().ColumnSpan(4).Padding(12).AlignCenter().Text("No se encontraron transacciones para este periodo.").FontColor("#778da9").Italic();
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Documento confidencial - Mi Banco Online").FontSize(9).FontColor("#778da9");
                        text.Span(" | Pagina ").FontSize(9).FontColor("#778da9");
                        text.CurrentPageNumber().FontSize(9).FontColor("#778da9");
                        text.Span(" de ").FontSize(9).FontColor("#778da9");
                        text.TotalPages().FontSize(9).FontColor("#778da9");
                    });
                });
            }).GeneratePdf();
        }
    }
}
