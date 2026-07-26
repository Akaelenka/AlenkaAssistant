using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlenkaAssistant.Models;

namespace AlenkaAssistant.Services
{
    /// <summary>
    /// Service for generating and printing purchase request invoices
    /// </summary>
    public class PrintService
    {
        private sealed class PrintLayoutOptions
        {
            public double PagePadding { get; init; }
            public double LogoHeight { get; init; }
            public double CompanyFontSize { get; init; }
            public double SubtitleFontSize { get; init; }
            public double AddressFontSize { get; init; }
            public double InvoiceTitleFontSize { get; init; }
            public double TableCellPadding { get; init; }
            public int MinTreatmentRows { get; init; }
            public double SectionSpacing { get; init; }
            public double SignatureTopMargin { get; init; }
        }

        /// <summary>
        /// Generate a FlowDocument for printing the invoice
        /// </summary>
        public FlowDocument GenerateInvoiceDocument(PurchaseRequestModel request, string patientName, string paperSize = "A4")
        {
            var layout = GetLayoutOptions(paperSize);
            var doc = new FlowDocument();

            var pageDimensions = GetPageDimensions(paperSize);
            doc.PageHeight = pageDimensions.Height;
            doc.PageWidth = pageDimensions.Width;
            doc.PagePadding = new Thickness(layout.PagePadding);
            doc.ColumnWidth = double.PositiveInfinity;
            doc.FontFamily = new FontFamily("Segoe UI");
            doc.FontSize = layout.AddressFontSize;

            doc.Blocks.Add(CreateHeader(layout));
            doc.Blocks.Add(CreateHeaderSeparator(layout));
            doc.Blocks.Add(CreateInvoiceTitle(layout));
            doc.Blocks.Add(CreatePatientInfo(request, patientName, layout));

            var sectionTitle = new Paragraph(new Run("Rincian Biaya Perawatan"))
            {
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, layout.SectionSpacing, 0, layout.SectionSpacing / 2)
            };
            doc.Blocks.Add(sectionTitle);

            doc.Blocks.Add(CreateTreatmentTable(request, layout));
            doc.Blocks.Add(CreateTotalSection(layout));
            doc.Blocks.Add(CreateSignatureSection(request, layout));

            return doc;
        }

        private struct PageDimensions
        {
            public double Width;
            public double Height;
        }

        private PageDimensions GetPageDimensions(string paperSize)
        {
            const double mmToPixels = 96.0 / 25.4;

            return paperSize.ToUpperInvariant() switch
            {
                "A4" => new PageDimensions { Width = 210 * mmToPixels, Height = 297 * mmToPixels },
                "A5" => new PageDimensions { Width = 148 * mmToPixels, Height = 210 * mmToPixels },
                "LETTER" => new PageDimensions { Width = 8.5 * 96, Height = 11 * 96 },
                "LEGAL" => new PageDimensions { Width = 8.5 * 96, Height = 14 * 96 },
                _ => new PageDimensions { Width = 210 * mmToPixels, Height = 297 * mmToPixels }
            };
        }

        private PrintLayoutOptions GetLayoutOptions(string paperSize)
        {
            return paperSize.ToUpperInvariant() switch
            {
                "A5" => new PrintLayoutOptions
                {
                    PagePadding = 18,
                    LogoHeight = 42,
                    CompanyFontSize = 13,
                    SubtitleFontSize = 9,
                    AddressFontSize = 8,
                    InvoiceTitleFontSize = 14,
                    TableCellPadding = 2,
                    MinTreatmentRows = 4,
                    SectionSpacing = 4,
                    SignatureTopMargin = 12
                },
                _ => new PrintLayoutOptions
                {
                    PagePadding = 32,
                    LogoHeight = 50,
                    CompanyFontSize = 16,
                    SubtitleFontSize = 11,
                    AddressFontSize = 10,
                    InvoiceTitleFontSize = 18,
                    TableCellPadding = 5,
                    MinTreatmentRows = 8,
                    SectionSpacing = 10,
                    SignatureTopMargin = 30
                }
            };
        }

        private Block CreateHeader(PrintLayoutOptions layout)
        {
            var headerGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 4)
            };

            var logo = CreateLogoImage(layout.LogoHeight);
            if (logo != null)
            {
                logo.VerticalAlignment = VerticalAlignment.Top;
                logo.HorizontalAlignment = HorizontalAlignment.Left;
                headerGrid.Children.Add(logo);
            }

            var companyPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            companyPanel.Children.Add(new TextBlock
            {
                Text = "ALENKA DENTAL CARE",
                FontSize = layout.CompanyFontSize,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            });

            companyPanel.Children.Add(new TextBlock
            {
                Text = "PRAKTIK DOKTER GIGI",
                FontSize = layout.SubtitleFontSize,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            });

            companyPanel.Children.Add(new TextBlock
            {
                Text = "drg. Novi Kurniawati\nJl. Pandu Dewonoto, RT 03. Pringgading, Guwosari, Pajangan, Bantul\nTelepon/ WA: 085 215 232 752",
                FontSize = layout.AddressFontSize,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            });

            headerGrid.Children.Add(companyPanel);

            return new BlockUIContainer(headerGrid);
        }

        private Image? CreateLogoImage(double height)
        {
            var logoPath = ResolveLogoPath();
            if (logoPath == null)
            {
                return null;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(logoPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                return new Image
                {
                    Source = bitmap,
                    Height = height,
                    Stretch = Stretch.Uniform
                };
            }
            catch
            {
                return null;
            }
        }

        private string? ResolveLogoPath()
        {
            var candidates = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "AlenkaLogo.png"),
                Path.Combine(AppContext.BaseDirectory, "Images", "AlenkaLogo.png")
            };

            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private Block CreateHeaderSeparator(PrintLayoutOptions layout)
        {
            return new Paragraph
            {
                BorderThickness = new Thickness(0, 2, 0, 0),
                BorderBrush = Brushes.Black,
                Margin = new Thickness(0, layout.SectionSpacing / 2, 0, layout.SectionSpacing)
            };
        }

        private Block CreateInvoiceTitle(PrintLayoutOptions layout)
        {
            var title = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, layout.SectionSpacing)
            };

            var invoiceText = new Run("INVOICE")
            {
                FontSize = layout.InvoiceTitleFontSize,
                FontWeight = FontWeights.Bold
            };
            title.Inlines.Add(invoiceText);

            return title;
        }

        private Block CreatePatientInfo(PurchaseRequestModel request, string patientName, PrintLayoutOptions layout)
        {
            var table = new Table
            {
                CellSpacing = 0,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 0, layout.SectionSpacing)
            };

            table.Columns.Add(new TableColumn { Width = new GridLength(100, GridUnitType.Pixel) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.0, GridUnitType.Star) });

            var rowGroup = new TableRowGroup();
            AddPatientInfoRow(rowGroup, "Nama", patientName ?? "-");
            AddPatientInfoRow(rowGroup, "No. RM", request.UserId ?? "-");
            AddPatientInfoRow(rowGroup, "Nama Dokter", GetDoctorDisplay(request) ?? "-");
            AddPatientInfoRow(rowGroup, "Waktu Cetak", request.CreatedAt.ToString("dd-MM-yyyy HH:mm:ss"));

            table.RowGroups.Add(rowGroup);
            return table;
        }

        private void AddPatientInfoRow(TableRowGroup rowGroup, string label, string value)
        {
            var row = new TableRow();

            var labelCell = new TableCell
            {
                Padding = new Thickness(0, 1, 8, 1),
                BorderThickness = new Thickness(0)
            };
            var labelPara = new Paragraph(new Run(label))
            {
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0),
                TextAlignment = TextAlignment.Left
            };
            labelCell.Blocks.Add(labelPara);
            row.Cells.Add(labelCell);

            var colonCell = new TableCell
            {
                Padding = new Thickness(0, 1, 8, 1),
                BorderThickness = new Thickness(0)
            };
            var colonPara = new Paragraph(new Run(":"))
            {
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0),
                TextAlignment = TextAlignment.Left
            };
            colonCell.Blocks.Add(colonPara);
            row.Cells.Add(colonCell);

            var valueCell = new TableCell
            {
                Padding = new Thickness(0, 1, 0, 1),
                BorderThickness = new Thickness(0)
            };
            var valuePara = new Paragraph(new Run(value))
            {
                Margin = new Thickness(0),
                TextAlignment = TextAlignment.Left
            };
            valueCell.Blocks.Add(valuePara);
            row.Cells.Add(valueCell);

            rowGroup.Rows.Add(row);
        }

        private Block CreateTreatmentTable(PurchaseRequestModel request, PrintLayoutOptions layout)
        {
            var table = new Table
            {
                CellSpacing = 0,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Margin = new Thickness(0, 0, 0, layout.SectionSpacing)
            };

            table.Columns.Add(new TableColumn { Width = new GridLength(2.5, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.0, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.0, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1.5, GridUnitType.Star) });

            var headerGroup = new TableRowGroup();
            var headerRow = new TableRow
            {
                Background = new SolidColorBrush(Color.FromRgb(170, 120, 80))
            };

            AddTableCell(headerRow, "Keterangan", true, false, layout.TableCellPadding);
            AddTableCell(headerRow, "Harga", true, true, layout.TableCellPadding);
            AddTableCell(headerRow, "Sebanyak", true, true, layout.TableCellPadding);
            AddTableCell(headerRow, "Jumlah", true, true, layout.TableCellPadding);

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            var bodyGroup = new TableRowGroup();
            if (request.CostDetails != null && request.CostDetails.Count > 0)
            {
                foreach (var cost in request.CostDetails)
                {
                    var dataRow = new TableRow();
                    if (bodyGroup.Rows.Count % 2 == 1)
                    {
                        dataRow.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                    }

                    string description = cost.TreatmentDesc ?? "";
                    //if (!string.IsNullOrWhiteSpace(cost.TreatmentType?.ToString()))
                    //{
                    //    description += $" ({TreatmentTypeHelper.GetDisplayName(cost.TreatmentType.Value)})";
                    //}

                    AddTableCell(dataRow, description, false, false, layout.TableCellPadding);
                    AddTableCell(dataRow, $"Rp {cost.Cost:N0}", false, true, layout.TableCellPadding);
                    AddTableCell(dataRow, $"Rp {cost.Discount:N0}", false, true, layout.TableCellPadding);

                    decimal jumlah = cost.Cost - cost.Discount;
                    AddTableCell(dataRow, $"Rp {jumlah:N0}", false, true, layout.TableCellPadding);

                    bodyGroup.Rows.Add(dataRow);
                }
            }

            int currentRows = bodyGroup.Rows.Count;
            for (int i = currentRows; i < layout.MinTreatmentRows; i++)
            {
                var emptyRow = new TableRow();
                if (i % 2 == 1)
                {
                    emptyRow.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                }

                AddTableCell(emptyRow, "", false, false, layout.TableCellPadding);
                AddTableCell(emptyRow, "Rp", false, true, layout.TableCellPadding);
                AddTableCell(emptyRow, "", false, true, layout.TableCellPadding);
                AddTableCell(emptyRow, "Rp", false, true, layout.TableCellPadding);

                bodyGroup.Rows.Add(emptyRow);
            }

            table.RowGroups.Add(bodyGroup);

            var totalGroup = new TableRowGroup();
            var totalRow = new TableRow
            {
                Background = new SolidColorBrush(Color.FromRgb(170, 120, 80))
            };

            AddTableCell(totalRow, "Jumlah Total", true, false, layout.TableCellPadding, 3);
            AddTableCell(totalRow, $"Rp {request.TotalCost:N0}", true, true, layout.TableCellPadding);

            totalGroup.Rows.Add(totalRow);
            table.RowGroups.Add(totalGroup);

            return table;
        }

        private Block CreateTotalSection(PrintLayoutOptions layout)
        {
            return new Paragraph
            {
                Margin = new Thickness(0, layout.SectionSpacing, 0, 0)
            };
        }

        private Block CreateSignatureSection(PurchaseRequestModel request, PrintLayoutOptions layout)
        {
            var section = new Paragraph
            {
                Margin = new Thickness(0, layout.SignatureTopMargin, 0, 0),
                TextAlignment = TextAlignment.Right
            };

            section.Inlines.Add(new Run(GetIndonesianDateString(request.CreatedAt)));
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new Run("Bagian Administrasi,"));
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new Run($"( {GetDoctorDisplay(request)} )"));

            return section;
        }

        private void AddTableCell(TableRow row, string text, bool isHeader, bool rightAlign, double padding, int columnSpan = 1)
        {
            var cell = new TableCell
            {
                Padding = new Thickness(padding),
                BorderThickness = new Thickness(0.5),
                BorderBrush = Brushes.Gray
            };

            if (columnSpan > 1)
            {
                cell.ColumnSpan = columnSpan;
            }

            var paragraph = new Paragraph(new Run(text))
            {
                Margin = new Thickness(0)
            };

            if (isHeader)
            {
                paragraph.Foreground = Brushes.White;
                paragraph.FontWeight = FontWeights.Bold;
            }

            if (rightAlign)
            {
                paragraph.TextAlignment = TextAlignment.Right;
            }

            cell.Blocks.Add(paragraph);
            row.Cells.Add(cell);
        }

        private string GetDoctorDisplay(PurchaseRequestModel request)
        {
            if (request.DoctorName == DoctorName.Other && !string.IsNullOrWhiteSpace(request.AltDoctorName))
            {
                return request.AltDoctorName;
            }

            if (!request.DoctorName.HasValue)
            {
                return "-";
            }

            return DoctorNameHelper.GetDisplayName(request.DoctorName.Value);
        }

        private string GetIndonesianDateString(DateTime dateTime)
        {
            string[] monthNames =
            {
                "Januari", "Februari", "Maret", "April", "Mei", "Juni",
                "Juli", "Agustus", "September", "Oktober", "November", "Desember"
            };

            return $"Bantul, {dateTime.Day} {monthNames[dateTime.Month - 1]} {dateTime.Year}";
        }
    }
}
