using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using AlenkaAssistant.Models;

namespace AlenkaAssistant.Services
{
    /// <summary>
    /// Service for generating and printing purchase request invoices
    /// </summary>
    public class PrintService
    {
        /// <summary>
        /// Generate a FlowDocument for printing the invoice
        /// </summary>
        public FlowDocument GenerateInvoiceDocument(PurchaseRequestModel request, string patientName, string paperSize = "A4")
        {
            var doc = new FlowDocument();

            // Set page dimensions based on paper size
            var pageDimensions = GetPageDimensions(paperSize);
            doc.PageHeight = pageDimensions.Height;
            doc.PageWidth = pageDimensions.Width;
            doc.PagePadding = new Thickness(40);
            doc.ColumnWidth = double.PositiveInfinity;

            // Header
            doc.Blocks.Add(CreateHeader());

            // Separator line after header
            doc.Blocks.Add(CreateHeaderSeparator());

            // Invoice Title
            doc.Blocks.Add(CreateInvoiceTitle());

            // Patient Information
            doc.Blocks.Add(CreatePatientInfo(request, patientName));

            // Spacing
            var spacing1 = new Paragraph();
            spacing1.Margin = new Thickness(0, 10, 0, 0);
            doc.Blocks.Add(spacing1);

            // Rincian Biaya Perawatan section title
            var sectionTitle = new Paragraph();
            sectionTitle.Margin = new Thickness(0, 0, 0, 10);
            var titleRun = new Run("Rincian Biaya Perawatan");
            titleRun.FontWeight = FontWeights.Bold;
            sectionTitle.Inlines.Add(titleRun);
            doc.Blocks.Add(sectionTitle);

            // Treatment Table
            doc.Blocks.Add(CreateTreatmentTable(request));

            // Total Amount
            doc.Blocks.Add(CreateTotalSection(request));

            // Signature Section
            doc.Blocks.Add(CreateSignatureSection(request));

            return doc;
        }

        private struct PageDimensions
        {
            public double Width;
            public double Height;
        }

        private PageDimensions GetPageDimensions(string paperSize)
        {
            // Convert mm to pixels (96 DPI = 3.78 pixels per mm)
            const double mmToPixels = 3.78;

            return paperSize.ToUpper() switch
            {
                "A4" => new PageDimensions { Width = 210 * mmToPixels, Height = 297 * mmToPixels },      // 210 x 297 mm
                "A5" => new PageDimensions { Width = 148 * mmToPixels, Height = 210 * mmToPixels },      // 148 x 210 mm
                "LETTER" => new PageDimensions { Width = 8.5 * 96, Height = 11 * 96 },                   // 8.5 x 11 inches
                "LEGAL" => new PageDimensions { Width = 8.5 * 96, Height = 14 * 96 },                    // 8.5 x 14 inches
                _ => new PageDimensions { Width = 210 * mmToPixels, Height = 297 * mmToPixels }          // Default to A4
            };
        }

        private Block CreateHeader()
        {
            var headerPanel = new System.Windows.Controls.StackPanel();
            headerPanel.HorizontalAlignment = HorizontalAlignment.Center;
            headerPanel.Margin = new Thickness(0, 0, 0, 10);

            // Try to load and display logo
            try
            {
                string logoPath = System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory, 
                    "Images", 
                    "AlenkaLogo.png"
                );

                if (System.IO.File.Exists(logoPath))
                {
                    var image = new System.Windows.Controls.Image();
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(logoPath, UriKind.Absolute);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    image.Source = bitmap;
                    image.Height = 50;
                    image.Width = 50;
                    image.Margin = new Thickness(0, 0, 0, 8);

                    headerPanel.Children.Add(image);
                }
            }
            catch
            {
                // If logo loading fails, just continue without it
            }

            // Company Name
            var companyName = new System.Windows.Controls.TextBlock();
            companyName.Text = "ALENKA DENTAL CARE";
            companyName.FontSize = 16;
            companyName.FontWeight = FontWeights.Bold;
            companyName.TextAlignment = TextAlignment.Center;
            headerPanel.Children.Add(companyName);

            // Subtitle
            var subtitle = new System.Windows.Controls.TextBlock();
            subtitle.Text = "PRAKTIK DOKTER GIGI";
            subtitle.FontSize = 11;
            subtitle.FontWeight = FontWeights.Bold;
            subtitle.TextAlignment = TextAlignment.Center;
            headerPanel.Children.Add(subtitle);

            // Address
            var address = new System.Windows.Controls.TextBlock();
            address.Text = "drg. Novi Kumarawati\nJl. Pandu Dewonoto, RT 03. Pringgazung, Guworak, Pajangan, Bantul\nTelepon/ WA: 085 215 232 752";
            address.FontSize = 10;
            address.TextAlignment = TextAlignment.Center;
            headerPanel.Children.Add(address);

            var header = new BlockUIContainer(headerPanel);
            return header;
        }

        private Block CreateHeaderSeparator()
        {
            var separator = new Paragraph();
            separator.BorderThickness = new Thickness(0, 2, 0, 0);
            separator.BorderBrush = Brushes.Black;
            separator.Margin = new Thickness(0, 10, 0, 15);
            separator.Padding = new Thickness(0, 0, 0, 0);
            return separator;
        }

        private Block CreateInvoiceTitle()
        {
            var title = new Paragraph();
            title.TextAlignment = TextAlignment.Center;
            title.Margin = new Thickness(0, 0, 0, 20);

            var invoiceText = new Run("INVOICE");
            invoiceText.FontSize = 18;
            invoiceText.FontWeight = FontWeights.Bold;
            title.Inlines.Add(invoiceText);

            return title;
        }

        private Block CreatePatientInfo(PurchaseRequestModel request, string patientName)
        {
            var table = new Table();
            table.CellSpacing = 0;
            table.BorderThickness = new Thickness(0);
            table.Margin = new Thickness(0, 0, 0, 20);

            // Two columns: label (fixed width) and value (flexible)
            table.Columns.Add(new TableColumn() { Width = new GridLength(120, GridUnitType.Pixel) });    // Labels
            table.Columns.Add(new TableColumn() { Width = new GridLength(1.0, GridUnitType.Star) });      // Values

            var rowGroup = new TableRowGroup();

            // Nama Row
            AddPatientInfoRow(rowGroup, "Nama", patientName ?? "-");

            // No. RM Row
            AddPatientInfoRow(rowGroup, "No. RM", request.UserId ?? "-");

            // Nama Dokter Row
            string doctorDisplay = GetDoctorDisplay(request);
            AddPatientInfoRow(rowGroup, "Nama Dokter", doctorDisplay ?? "-");

            // Waktu Cetak Row
            string printTime = request.CreatedAt.ToString("dd-MM-yyyy HH:mm:ss");
            AddPatientInfoRow(rowGroup, "Waktu Cetak", printTime);

            table.RowGroups.Add(rowGroup);

            // Add spacing after patient info
            var container = new BlockUIContainer();
            var spacer = new System.Windows.Controls.Grid();
            spacer.Height = 15;
            container.Child = spacer;

            // Create a paragraph after the table for spacing
            var spacing = new Paragraph();
            spacing.Margin = new Thickness(0, 0, 0, 0);

            // We need to return just the table, not multiple blocks
            // So we'll add the spacing info section after it in the main method
            return table;
        }

        private void AddPatientInfoRow(TableRowGroup rowGroup, string label, string value)
        {
            var row = new TableRow();

            // Label cell with colon
            var labelCell = new TableCell();
            labelCell.Padding = new Thickness(0, 3, 8, 3);
            labelCell.BorderThickness = new Thickness(0);
            var labelPara = new Paragraph(new Run(label));
            labelPara.FontWeight = FontWeights.Bold;
            labelPara.Margin = new Thickness(0);
            labelPara.TextAlignment = TextAlignment.Left;
            labelCell.Blocks.Add(labelPara);
            row.Cells.Add(labelCell);

            // Colon cell
            var colonCell = new TableCell();
            colonCell.Padding = new Thickness(0, 3, 8, 3);
            colonCell.BorderThickness = new Thickness(0);
            var colonPara = new Paragraph(new Run(":"));
            colonPara.FontWeight = FontWeights.Bold;
            colonPara.Margin = new Thickness(0);
            colonPara.TextAlignment = TextAlignment.Left;
            colonCell.Blocks.Add(colonPara);
            row.Cells.Add(colonCell);

            // Value cell
            var valueCell = new TableCell();
            valueCell.Padding = new Thickness(0, 3, 0, 3);
            valueCell.BorderThickness = new Thickness(0);
            var valuePara = new Paragraph(new Run(value));
            valuePara.Margin = new Thickness(0);
            valuePara.TextAlignment = TextAlignment.Left;
            valueCell.Blocks.Add(valuePara);
            row.Cells.Add(valueCell);

            rowGroup.Rows.Add(row);
        }

        private Block CreateTreatmentTable(PurchaseRequestModel request)
        {
            var table = new Table();
            table.CellSpacing = 0;
            table.BorderThickness = new Thickness(1);
            table.BorderBrush = Brushes.Black;
            table.Margin = new Thickness(0, 0, 0, 20);

            // Column definitions (using Star unit for relative widths)
            table.Columns.Add(new TableColumn() { Width = new GridLength(2.5, GridUnitType.Star) }); // Keterangan
            table.Columns.Add(new TableColumn() { Width = new GridLength(1.0, GridUnitType.Star) }); // Harga
            table.Columns.Add(new TableColumn() { Width = new GridLength(1.0, GridUnitType.Star) }); // Sebanyak (Discount)
            table.Columns.Add(new TableColumn() { Width = new GridLength(1.5, GridUnitType.Star) }); // Jumlah

            // Header Row
            var headerGroup = new TableRowGroup();
            var headerRow = new TableRow();
            headerRow.Background = new SolidColorBrush(Color.FromRgb(170, 120, 80)); // Brown color from template

            AddTableCell(headerRow, "Keterangan", true, true);
            AddTableCell(headerRow, "Harga", true, true);
            AddTableCell(headerRow, "Sebanyak", true, true);
            AddTableCell(headerRow, "Jumlah", true, true);

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            // Data Rows
            var bodyGroup = new TableRowGroup();
            if (request.CostDetails != null && request.CostDetails.Count > 0)
            {
                foreach (var cost in request.CostDetails)
                {
                    var dataRow = new TableRow();

                    // Alternating row colors for better readability
                    if (bodyGroup.Rows.Count % 2 == 1)
                    {
                        dataRow.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                    }

                    string description = cost.TreatmentDesc ?? "";
                    if (!string.IsNullOrWhiteSpace(cost.TreatmentType?.ToString()))
                    {
                        description += $" ({TreatmentTypeHelper.GetDisplayName(cost.TreatmentType.Value)})";
                    }

                    AddTableCell(dataRow, description, false, false);
                    AddTableCell(dataRow, $"Rp {cost.Cost:N0}", false, true); // Right-aligned
                    AddTableCell(dataRow, $"Rp {cost.Discount:N0}", false, true); // Right-aligned

                    decimal jumlah = cost.Cost - cost.Discount;
                    AddTableCell(dataRow, $"Rp {jumlah:N0}", false, true); // Right-aligned

                    bodyGroup.Rows.Add(dataRow);
                }
            }

            // Add empty rows if less than 8 detail rows
            int currentRows = bodyGroup.Rows.Count;
            for (int i = currentRows; i < 8; i++)
            {
                var emptyRow = new TableRow();
                if (i % 2 == 1)
                {
                    emptyRow.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                }

                AddTableCell(emptyRow, "", false, false);
                AddTableCell(emptyRow, "Rp", false, true);
                AddTableCell(emptyRow, "", false, true);
                AddTableCell(emptyRow, "Rp", false, true);

                bodyGroup.Rows.Add(emptyRow);
            }

            table.RowGroups.Add(bodyGroup);

            // Total Row
            var totalGroup = new TableRowGroup();
            var totalRow = new TableRow();
            totalRow.Background = new SolidColorBrush(Color.FromRgb(170, 120, 80)); // Brown color

            AddTableCell(totalRow, "Jumlah Total", true, false, 3); // Span 3 columns
            AddTableCell(totalRow, $"Rp {request.TotalCost:N0}", true, true);

            totalGroup.Rows.Add(totalRow);
            table.RowGroups.Add(totalGroup);

            return table;
        }

        private Block CreateTotalSection(PurchaseRequestModel request)
        {
            var spacer = new Paragraph();
            spacer.Margin = new Thickness(0, 20, 0, 0);
            return spacer;
        }

        private Block CreateSignatureSection(PurchaseRequestModel request)
        {
            var section = new Paragraph();
            section.Margin = new Thickness(0, 40, 0, 0);

            // Bantul date
            var dateString = GetIndonesianDateString(request.CreatedAt);
            section.Inlines.Add(new Run(dateString));
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());

            // Bagian Administrasi
            var adminLabel = new Run("Bagian Administrasi,");
            section.Inlines.Add(adminLabel);
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());
            section.Inlines.Add(new LineBreak());

            // Signature line for doctor
            string doctorDisplay = GetDoctorDisplay(request);
            var doctorSignature = new Run($"( {doctorDisplay} )");
            section.Inlines.Add(doctorSignature);

            return section;
        }

        private void AddTableCell(TableRow row, string text, bool isHeader, bool rightAlign, int columnSpan = 1)
        {
            var cell = new TableCell();
            cell.Padding = new Thickness(5);
            cell.BorderThickness = new Thickness(0.5);
            cell.BorderBrush = Brushes.Gray;
            if (columnSpan > 1)
            {
                cell.ColumnSpan = columnSpan;
            }

            var paragraph = new Paragraph(new Run(text));
            if (isHeader)
            {
                paragraph.Foreground = Brushes.White;
                paragraph.FontWeight = FontWeights.Bold;
            }
            if (rightAlign)
            {
                paragraph.TextAlignment = TextAlignment.Right;
            }
            paragraph.Margin = new Thickness(0);

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
                return "-";

            return DoctorNameHelper.GetDisplayName(request.DoctorName.Value);
        }

        private string GetIndonesianDateString(DateTime dateTime)
        {
            string[] monthNames = {
                "Januari", "Februari", "Maret", "April", "Mei", "Juni",
                "Juli", "Agustus", "September", "Oktober", "November", "Desember"
            };

            string city = "Bantul";
            string day = dateTime.Day.ToString();
            string month = monthNames[dateTime.Month - 1];
            string year = dateTime.Year.ToString();

            return $"{city}, {day} {month} {year}";
        }
    }
}
