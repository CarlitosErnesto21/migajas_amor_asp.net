using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Elements;
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
                page.PageColor("#fff"); // Soft pastel background
                page.DefaultTextStyle(x => x.FontSize(9).FontColor("#4d375d").FontFamily("Arial"));

                // Header
                page.Header()
                    .PaddingBottom(10)
                    .Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Migajas de Amor").FontSize(16).Bold().FontColor("#b5838d");
                            col.Item().Text("Detalle de pedidos")
                                .SemiBold()
                                .FontSize(24)
                                .FontColor("#4361ee");
                        });
                    });

                // Content
                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Text($"Total de pedidos: {Model.Count}")
                            .FontSize(11)
                            .FontColor("#f4845f")
                            .Bold();

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
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("PRECIO").FontSize(9);
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("CANTIDAD").FontSize(9);
                                header.Cell().Element(CellStyleHeader).AlignCenter().Text("TOTAL").FontSize(9);

                                static IContainer CellStyleHeader(IContainer container) =>
                                    container.DefaultTextStyle(x => x.SemiBold().FontSize(9).FontColor("#fff"))
                                             .Background("#b5838d")
                                             .PaddingVertical(8)
                                             .PaddingHorizontal(4)
                                             .BorderBottom(2)
                                             .BorderColor("#f7b2ad")
                                             .AlignMiddle();
                            });

                            // Rows
                            bool alternate = false;
                            foreach (var item in Model)
                            {
                                var bgColor = alternate ? "#f7e2de" : "#fff6f3";
                                alternate = !alternate;

                                table.Cell().Element(c => CellStyleRow(c, bgColor)).AlignCenter().Text(item.FechaPedido.ToString("yyyy-MM-dd"));
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).Text(item.Nombre);
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).Text(item.Email);
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).Text(item.Estado);
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).Text(item.Producto);
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).Row(row =>
                                {
                                    row.RelativeItem().Text("$").FontColor("#b5838d").AlignLeft();
                                    row.RelativeItem().Text(item.PrecioUnitario.ToString("F2")).AlignRight();
                                });
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).AlignCenter().Text(item.Cantidad.ToString());
                                table.Cell().Element(c => CellStyleRow(c, bgColor)).Row(row =>
                                {
                                    row.RelativeItem().Text("$").FontColor("#b5838d").AlignLeft();
                                    row.RelativeItem().Text(item.Total.ToString("F2")).AlignRight();
                                });

                                static IContainer CellStyleRow(IContainer container, string background) =>
                                    container
                                        .Background(background)
                                        .PaddingVertical(4)
                                        .PaddingHorizontal(4)
                                        .BorderBottom(1)
                                        .BorderColor("#f7b2ad")
                                        .AlignMiddle();
                            }
                        });
                    });

                // Footer (corregido)
                page.Footer()
                    .AlignCenter()
                    .Element(e =>
                        e.PaddingTop(10)
                         .Text(x =>
                         {
                             x.Span("Página ").FontSize(10).FontColor("#b5838d");
                             x.CurrentPageNumber().FontSize(10).FontColor("#b5838d");
                             x.Span(" de ").FontSize(10).FontColor("#b5838d");
                             x.TotalPages().FontSize(10).FontColor("#b5838d");
                         })
                    );
            });
        }
    }
}