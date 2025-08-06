using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;

namespace migajas_amor.app.Pdf
{
    public class DetallePedidoPdfDoc : IDocument
    {
        private List<DetallePedidoPdf> Model { get; }

        public DetallePedidoPdfDoc(List<DetallePedidoPdf> model)
        {
            Model = model;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header()
                    .Text("Detalle de pedidos")
                    .SemiBold().FontSize(22).FontColor(Colors.Blue.Medium);

                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text("Total pedidos: " + Model.Count)
                            .FontSize(10);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // FechaPedido
                                columns.RelativeColumn(1); // Nombre
                                columns.RelativeColumn(2); // Email
                                columns.RelativeColumn(1); // Estado
                                columns.RelativeColumn(1); // Producto
                                columns.RelativeColumn(1); // PrecioUnitario
                                columns.RelativeColumn(1); // Cantidad
                                columns.RelativeColumn(1); // Total
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("FECHA");
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("CLIENTE");
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("CORREO ELECTRÓNICO");
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("ESTADO");
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("PRODUCTO");
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("PRECIO").FontSize(8);
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("CANTIDAD").FontSize(8);
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("TOTAL").FontSize(8);

                                static IContainer CellStyleHeader(IContainer container) =>
                                    container.DefaultTextStyle(x => x.SemiBold().FontSize(8).FontColor(Colors.White))
                                             .Background(Colors.Blue.Medium)
                                             .PaddingVertical(7)
                                             .PaddingHorizontal(3)
                                             .BorderBottom(1)
                                             .BorderColor(Colors.Grey.Medium);
                            });

                            // Rows
                            foreach (var item in Model)
                            {
                                table.Cell().Element(CellStyleRow).AlignCenter().Text(item.FechaPedido.ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyleRow).Text(item.Nombre);
                                table.Cell().Element(CellStyleRow).Text(item.Email);
                                table.Cell().Element(CellStyleRow).Text(item.Estado);
                                table.Cell().Element(CellStyleRow).Text(item.Producto);
                                table.Cell().Element(CellStyleRow).Row(row =>
                                {
                                    row.RelativeItem().Text("$").AlignLeft();
                                    row.RelativeItem().Text(item.PrecioUnitario.ToString("F2")).AlignRight();
                                });
                                table.Cell().Element(CellStyleRow).AlignCenter().Text(item.Cantidad.ToString());
                                table.Cell().Element(CellStyleRow).Row(row =>
                                {
                                    row.RelativeItem().Text("$").AlignLeft();
                                    row.RelativeItem().Text(item.Total.ToString("F2")).AlignRight();
                                });

                                static IContainer CellStyleRow(IContainer container) =>
                                    container.PaddingVertical(2).PaddingHorizontal(3);
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ").FontSize(9);
                        x.CurrentPageNumber().FontSize(9);
                    });
            });
        }
    }
}