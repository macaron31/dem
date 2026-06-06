using Microsoft.Data.Sqlite;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfApp.Models
{

    internal class TableModel
    {
        public string Name { get; set; }
        public List<string> Columns { get; set; } = new List<string>();
        public List<string> ForeignKeys { get; set; } = new List<string>();
    }

    internal class CreateDataBase
    {
        private readonly string dbPath;
        private readonly string dbName = Config.DataBaseName;

        public CreateDataBase()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dbPath = Path.Combine(appDirectory, dbName);

            if (!File.Exists(dbPath))
            {
                CreateDatabase();
            }
        }

        private void CreateDatabase()
        {
            try
            {
                var connection = new SqliteConnection($"Data Source={dbPath}");
                connection.Open();

                EnableForeignKeys(connection);

                new SqliteCommand($@"
                CREATE TABLE IF NOT EXISTS {Config.Table0Name} (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    {Config.Table0Attribute1} TEXT NOT NULL,
                    {Config.Table0Attribute2} TEXT NOT NULL,
                    {Config.Table0Attribute3} TEXT NOT NULL
                );", connection).ExecuteNonQuery();

                new SqliteCommand($@"
                CREATE TABLE IF NOT EXISTS {Config.Table1Name} (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    {Config.Table1Attribute1} TEXT NOT NULL,
                    {Config.Table1Attribute2} TEXT,
                    {Config.Table1Attribute3} TEXT,
                    {Config.Table1Attribute4} INTEGER NOT NULL DEFAULT 1
                );", connection).ExecuteNonQuery();

                new SqliteCommand($@"
                CREATE TABLE IF NOT EXISTS {Config.Table2Name} (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    {Config.Table2Attribute1} TEXT NOT NULL,
                    {Config.Table2Attribute2} REAL NOT NULL CHECK ({Config.Table2Attribute2} >= 0),
                    {Config.Table2Attribute3} TEXT,
                    {Config.Table3Attribute1} INTEGER NOT NULL DEFAULT 1, 
                    FOREIGN KEY ({Config.Table3Attribute1}) REFERENCES {Config.Table1Name}(ID) ON DELETE RESTRICT
                );", connection).ExecuteNonQuery();

                new SqliteCommand($@"
                CREATE TABLE IF NOT EXISTS {Config.Table3Name} (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    {Config.Table3Attribute1} INTEGER NOT NULL,
                    {Config.Table3Attribute2} TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    {Config.Table3Attribute3} TEXT NOT NULL DEFAULT 'New',
                    {Config.Table3Attribute4} TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY ({Config.Table3Attribute1}) REFERENCES {Config.Table1Name}(ID) ON DELETE RESTRICT
                );", connection).ExecuteNonQuery();

                new SqliteCommand($@"
                CREATE TABLE IF NOT EXISTS {Config.Table4Name} (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    {Config.Table4Attribute1} INTEGER NOT NULL,
                    {Config.Table4Attribute2} INTEGER NOT NULL,
                    {Config.Table4Attribute3} INTEGER NOT NULL CHECK ({Config.Table4Attribute3} > 0),
                    {Config.Table4Attribute4} REAL NOT NULL CHECK ({Config.Table4Attribute4} >= 0),

                    FOREIGN KEY ({Config.Table4Attribute1}) REFERENCES {Config.Table3Name}(ID) ON DELETE CASCADE,
                    FOREIGN KEY ({Config.Table4Attribute2}) REFERENCES {Config.Table2Name}(ID) ON DELETE RESTRICT
                );", connection).ExecuteNonQuery();

                string pdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ER_Diagram.pdf");
                GenerateErDiagramPdf(connection, pdfPath);

                connection.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка создания БД:\n{ex.Message}");
            }
        }

        private void EnableForeignKeys(SqliteConnection connection)
        {
            var command = new SqliteCommand("PRAGMA foreign_keys = ON;", connection);
            command.ExecuteNonQuery();
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={dbPath}");
        }

        private List<TableModel> ExtractSchemaFromSqlite(SqliteConnection connection)
        {
            var tables = new List<TableModel>();

            // Получаем список таблиц через прямой SQL-запрос к sqlite_master
            using (var cmd = new SqliteCommand(@"
        SELECT name FROM sqlite_master 
        WHERE type='table' 
        AND name NOT LIKE 'sqlite_%' 
        ORDER BY name;", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string tableName = reader["name"].ToString();

                    var tableModel = new TableModel { Name = tableName };

                    // Получаем информацию о столбцах (PRAGMA table_info)
                    using (var cmdCols = new SqliteCommand($"PRAGMA table_info({tableName});", connection))
                    using (var readerCols = cmdCols.ExecuteReader())
                    {
                        while (readerCols.Read())
                        {
                            string colName = readerCols["name"].ToString();
                            string colType = readerCols["type"].ToString();
                            bool isPk = Convert.ToInt32(readerCols["pk"]) > 0;

                            string pkMarker = isPk ? "🔑 " : "    ";
                            tableModel.Columns.Add($"{pkMarker}{colName} ({colType})");
                        }
                    }

                    // Получаем информацию о внешних ключах (PRAGMA foreign_key_list)
                    using (var cmdFk = new SqliteCommand($"PRAGMA foreign_key_list({tableName});", connection))
                    using (var readerFk = cmdFk.ExecuteReader())
                    {
                        while (readerFk.Read())
                        {
                            string targetTable = readerFk["table"].ToString();
                            tableModel.ForeignKeys.Add(targetTable);
                        }
                    }

                    tables.Add(tableModel);
                }
            }

            return tables;
        }



        private void GenerateErDiagramPdf(SqliteConnection connection, string outputPath)
        {
            PdfSharp.Fonts.GlobalFontSettings.UseWindowsFontsUnderWindows = true;
            List<TableModel> tables = ExtractSchemaFromSqlite(connection);

            // Параметры отрисовки
            const double tableWidth = 200;
            const double columnHeight = 18;
            const double margin = 10;
            const double minDistance = 10; // минимальное расстояние между таблицами

            var document = new PdfDocument();
            var page = document.AddPage();

            // Шрифты и стили
            var titleFont = new XFont("Arial", 12, XFontStyleEx.Bold);
            var font = new XFont("Arial", 9, XFontStyleEx.Regular);
            var tableStroke = new XPen(XColors.CadetBlue, 2);
            var edgeStroke = new XPen(XColors.SlateGray, 1.5);
            var tableBg = XBrushes.GhostWhite;
            var arrowPen = new XPen(XColors.SlateGray, 2);

            // Собираем информацию о связях
            var foreignKeys = new List<(string FromTable, string FromColumn, string ToTable, string ToColumn)>();
            foreach (var table in tables)
            {
                using (var cmd = new SqliteCommand($"PRAGMA foreign_key_list({table.Name});", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreignKeys.Add((
                            FromTable: table.Name,
                            FromColumn: reader["from"].ToString(),
                            ToTable: reader["table"].ToString(),
                            ToColumn: reader["to"].ToString()
                        ));
                    }
                }
            }

            // Хаотичное размещение с улучшенным распределением
            var random = new Random();
            double totalWidth = 1000;  // увеличенная ширина для лучшего размещения
            double totalHeight = 600; // увеличенная высота
            page.Width = XUnit.FromPoint(totalWidth);
            page.Height = XUnit.FromPoint(totalHeight);

            var gfx = XGraphics.FromPdfPage(page);
            var positionMap = new Dictionary<string, (double X, double Y)>();

            // Размещаем таблицы, избегая пересечений
            foreach (var table in tables)
            {
                bool validPositionFound = false;
                double layerX = 0, layerY = 0;
                int maxAttempts = 500;

                for (int attempt = 0; attempt < maxAttempts && !validPositionFound; attempt++)
                {
                    layerX = margin + random.NextDouble() * (totalWidth - margin * 2 - tableWidth);
                    layerY = margin + random.NextDouble() * (totalHeight - margin * 2 - 150);

                    // Проверяем пересечение с уже размещёнными таблицами
                    bool overlaps = false;
                    foreach (var placed in positionMap)
                    {
                        var (placedX, placedY) = placed.Value;
                        double placedWidth = tableWidth;
                        double placedHeight = 35 + tables.First(t => t.Name == placed.Key).Columns.Count * columnHeight;

                        if (layerX < placedX + placedWidth + minDistance &&
                            layerX + tableWidth > placedX - minDistance &&
                            layerY < placedY + placedHeight + minDistance &&
                            layerY + (35 + table.Columns.Count * columnHeight) > placedY - minDistance)
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (!overlaps)
                    {
                        validPositionFound = true;
                    }
                }

                // Высота таблицы зависит от количества столбцов
                double tableHeight = 35 + table.Columns.Count * columnHeight;
                positionMap[table.Name] = (layerX, layerY);

                // Отрисовка таблицы
                gfx.DrawRectangle(tableStroke, tableBg, layerX, layerY, tableWidth, tableHeight);
                gfx.DrawString(table.Name, titleFont, XBrushes.DarkSlateGray,
                    new XRect(layerX + 8, layerY + 6, tableWidth - 16, 20),
                    XStringFormats.TopLeft);
                gfx.DrawLine(tableStroke, layerX, layerY + 24, layerX + tableWidth, layerY + 24);

                double currentY = layerY + 38;
                foreach (var column in table.Columns)
                {
                    gfx.DrawString(column, font, XBrushes.Black,
                        new XRect(layerX + 10, currentY, tableWidth - 20, columnHeight),
                        XStringFormats.TopLeft);
                    currentY += columnHeight;
                }
            }

            // Рисуем связи с поворотами на 90 градусов (L-образные линии)
            // Рисуем связи с поворотами на 90 градусов (L-образные линии), доходящие до границ таблиц
            foreach (var fk in foreignKeys)
            {
                if (positionMap.TryGetValue(fk.FromTable, out var fromPos) &&
                    positionMap.TryGetValue(fk.ToTable, out var toPos))
                {
                    var fromTable = tables.First(t => t.Name == fk.FromTable);
                    var toTable = tables.First(t => t.Name == fk.ToTable);

                    // Высота таблиц зависит от количества столбцов
                    double fromTableHeight = 35 + fromTable.Columns.Count * columnHeight;
                    double toTableHeight = 35 + toTable.Columns.Count * columnHeight;

                    // Определяем точки на границах таблиц для начала и конца связи
                    // Для простоты используем верхнюю границу (можно адаптировать под другие стороны)
                    double fromX = fromPos.X + tableWidth / 2;
                    double fromY = fromPos.Y; // Верхняя граница исходной таблицы
                    double toX = toPos.X + tableWidth / 2;
                    double toY = toPos.Y + toTableHeight; // Нижняя граница целевой таблицы

                    var path = new XGraphicsPath();

                    // Эвристика: если разница по X больше, сначала рисуем горизонталь
                    if (Math.Abs(fromX - toX) >= Math.Abs(fromY - toY))
                    {
                        // Сначала горизонтальный сегмент
                        path.AddLine(fromX, fromY, toX, fromY);
                        path.AddLine(toX, fromY, toX, toY);
                    }
                    else
                    {
                        // Сначала вертикальный сегмент
                        path.AddLine(fromX, fromY, fromX, toY);
                        path.AddLine(fromX, toY, toX, toY);
                    }

                    gfx.DrawPath(edgeStroke, path);

                    // Рисуем стрелку на конце (у целевой таблицы)
                    DrawArrowAtEnd(gfx, toX, toY, fromX, fromY, arrowPen);

                    // Подпись связи
                    string label = $"{fk.FromColumn} → {fk.ToColumn}";
                    double labelX = (fromX + toX) / 2;
                    double labelY = (fromY + toY) / 2 - 10;

                    gfx.DrawString(label, font, XBrushes.SlateGray,
                        new XRect(labelX - 40, labelY, 80, 15),
                        XStringFormats.Center);
                }
            }


            document.Save(outputPath);
        }

        // Улучшенная отрисовка стрелки — учитывает направление линии
        private void DrawArrowAtEnd(XGraphics gfx, double endX, double endY, double startX, double startY, XPen pen)
        {
            // Вычисляем направление последнего сегмента линии
            double dx = endX - startX;
            double dy = endY - startY;

            // Нормализуем вектор
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length == 0) return;
            dx /= length;
            dy /= length;

            const double arrowSize = 10;

            // Координаты концов «крыльев» стрелки
            double arrowX1 = endX - arrowSize * (dx - dy * 0.5);
            double arrowY1 = endY - arrowSize * (dy + dx * 0.5);

            double arrowX2 = endX - arrowSize * (dx + dy * 0.5);
            double arrowY2 = endY - arrowSize * (dy - dx * 0.5);

            gfx.DrawLine(pen, endX, endY, arrowX1, arrowY1);
            gfx.DrawLine(pen, endX, endY, arrowX2, arrowY2);
        }
    }
}