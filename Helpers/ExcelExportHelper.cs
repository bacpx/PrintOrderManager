using ClosedXML.Excel;
using PrintOrderManager.Models;
using System.Collections.Generic;
using System.IO;

namespace PrintOrderManager.Helpers
{
    public static class ExcelExportHelper
    {
        public static void ExportOrders(IEnumerable<Order> orders)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachDonHang");

                // Title
                worksheet.Cell(1, 1).Value = "DANH SÁCH ĐƠN HÀNG IN ẤN";
                worksheet.Range("A1:G1").Merge().Style.Font.SetBold().Font.FontSize = 16;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Headers
                int row = 3;
                worksheet.Cell(row, 1).Value = "Ngày";
                worksheet.Cell(row, 2).Value = "Nội dung";
                worksheet.Cell(row, 3).Value = "Kích thước";
                worksheet.Cell(row, 4).Value = "Chất liệu";
                worksheet.Cell(row, 5).Value = "Gia công";
                worksheet.Cell(row, 6).Value = "Số lượng";
                worksheet.Cell(row, 7).Value = "Đơn giá";
                worksheet.Cell(row, 8).Value = "Thành tiền";

                var headerRange = worksheet.Range(row, 1, row, 8);
                headerRange.Style.Font.SetBold().Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                row++;
                foreach (var order in orders)
                {
                    if (order.OrderItems == null || order.OrderItems.Count == 0) continue;

                    int startRow = row;
                    foreach (var item in order.OrderItems)
                    {
                        worksheet.Cell(row, 1).Value = order.OrderDate.ToString("dd/MM/yyyy");
                        worksheet.Cell(row, 2).Value = item.ProductName;
                        worksheet.Cell(row, 3).Value = item.Size;
                        worksheet.Cell(row, 4).Value = item.Material?.Name;
                        worksheet.Cell(row, 5).Value = item.Process?.Name;
                        worksheet.Cell(row, 6).Value = item.Quantity;
                        worksheet.Cell(row, 7).Value = item.UnitPrice;
                        
                        worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";

                        row++;
                    }

                    // Merge and format order total
                    int endRow = row - 1;
                    if (startRow <= endRow)
                    {
                        var amountCell = worksheet.Cell(startRow, 8);
                        amountCell.Value = order.TotalAmount;
                        amountCell.Style.NumberFormat.Format = "#,##0";
                        amountCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        
                        if (startRow < endRow)
                        {
                            worksheet.Range(startRow, 8, endRow, 8).Merge();
                        }
                    }
                }

                // Add borders
                if (row > 4)
                {
                    worksheet.Range(4, 1, row - 1, 8).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    worksheet.Range(4, 1, row - 1, 8).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                }

                worksheet.Columns().AdjustToContents();

                var defaultDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
                if (!Directory.Exists(defaultDir))
                {
                    Directory.CreateDirectory(defaultDir);
                }

                var filePath = Path.Combine(defaultDir, $"DonHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                workbook.SaveAs(filePath);

                System.Windows.MessageBox.Show($"Xuất Excel thành công!\nFile: {filePath}", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}
