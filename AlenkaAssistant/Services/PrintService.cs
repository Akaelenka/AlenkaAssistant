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
        public FlowDocument GenerateInvoiceDocument(PurchaseRequestModel request, string patientName)
        {
            var doc = new FlowDocument();
            doc.PageHeight = 11.5 * 96; // 11.5 inches in pixels (96 DPI)
            doc.PageWidth = 8.5 * 96;   // 8.5 inches in pixels
            doc.PagePadding = new Thickness(40);
            doc.ColumnWidth = double.PositiveInfinity;

            // Header
            doc.Blocks.Add(CreateHeader());

            // Invoice Title
            doc.Blocks.Add(CreateInvoiceTitle());

            // Patient Information
            doc.Blocks.Add(CreatePatientInfo(request, patientName));

            // Treatment Section Title
            doc.Blocks.Add(CreateTreatmentSectionTitle());

            // Treatment Table
            doc.Blocks.Add(CreateTreatmentTable(request));

            // Total Amount
            doc.Blocks.Add(CreateTotalSection(request));

            // Signature Section
            doc.Blocks.Add(CreateSignatureSection(request));

            return doc;
        }

        private Block CreateHeader()
        {
            var header = new Paragraph();
            header.TextAlignment = TextAlignment.Center;
            header.Margin = new Thickness(0, 0, 0, 10);

            // Company Name
            var companyName = new Run("ALENKA DENTAL CARE");
            companyName.FontSize = 16;
            companyName.FontWeight = FontWeights.Bold;
            header.Inlines.Add(companyName);

            header.Inlines.Add(new LineBreak());

            // Subtitle
            var subtitle = new Run("PRAKTIK DOKTER GIGI");
            subtitle.FontSize = 11;
            subtitle.FontWeight = FontWeights.Bold;
            header.Inlines.Add(subtitle);

            header.Inlines.Add(new LineBreak());

            // Address
            var address = new Run("drg. Novi Kumarawati\nJl. Pandu Dewonoto, RT 03. Pringgazung, Guworak, Pajangan, Bantul\nTelepon/ WA: 085 215 232 752");
            address.FontSize = 10;
            header.Inlines.Add(address);

            header.Inlines.Add(new LineBreak());

            // Separator line
            var separator = new Paragraph();
            separator.BorderThickness = new Thickness(0, 1, 0, 0);
            separator.BorderBrush = Brushes.Black;
            separator.Margin = new Thickness(0, 10, 0, 10);

            var container = new BlockUIContainer();
            return header;
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
            var infoPanel = new Paragraph();
            infoPanel.Margin = new Thickness(0, 0, 0, 20);

            // Nama
            var namaLabel = new Run("Nama                    : ");
            namaLabel.FontWeight = FontWeights.Bold;
            infoPanel.Inlines.Add(namaLabel);
            infoPanel.Inlines.Add(new Run(patientName ?? "-"));
            infoPanel.Inlines.Add(new LineBreak());

            // No. RM
            var rmLabel = new Run("No. RM                 : ");
            rmLabel.FontWeight = FontWeights.Bold;
            infoPanel.Inlines.Add(rmLabel);
            infoPanel.Inlines.Add(new Run(request.UserId ?? "-"));
            infoPanel.Inlines.Add(new LineBreak());

            // Nama Dokter
            var doctorLabel = new Run("Nama Dokter         : ");
            doctorLabel.FontWeight = FontWeights.Bold;
            infoPanel.Inlines.Add(doctorLabel);
            string doctorDisplay = GetDoctorDisplay(request);
            infoPanel.Inlines.Add(new Run(doctorDisplay ?? "-"));
            infoPanel.Inlines.Add(new LineBreak());

            // Waktu Cetak
            var printTimeLabel = new Run("Waktu Cetak          : ");
            printTimeLabel.FontWeight = FontWeights.Bold;
            infoPanel.Inlines.Add(printTimeLabel);
            string printTime = request.CreatedAt.ToString("dd-MM-yyyy HH:mm:ss");
            infoPanel.Inlines.Add(new Run(printTime));

            infoPanel.Inlines.Add(new LineBreak());
            infoPanel.Inlines.Add(new LineBreak());

            // Rincian Biaya Perawatan
            var sectionTitle = new Run("Rincian Biaya Perawatan");
            sectionTitle.FontWeight = FontWeights.Bold;
            infoPanel.Inlines.Add(sectionTitle);

            return infoPanel;
        }

        private Block CreateTreatmentSectionTitle()
        {
            var spacer = new Paragraph();
            spacer.Margin = new Thickness(0, 0, 0, 10);
            return spacer;
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
