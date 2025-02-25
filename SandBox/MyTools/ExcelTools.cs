using ClosedXML.Excel;

namespace ExcelFluentTools
{
    public class ExcelTools
    {
        private XLWorkbook? _workbook; // Pour stocker le classeur
        private IXLWorksheet? _worksheet; // Pour stocker la feuille de calcul
        private List<int> _columnsIndexes = new List<int>(); // Pour stocker les index des colonnes

        // Définir le classeur
        public ExcelTools DefinirWorkBook(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Le fichier {path} n'a pas été trouvé.");

            _workbook = new XLWorkbook(path);
            return this; // Retourne l'instance actuelle pour le chaînage fluent
        }

        // Définir la feuille de calcul
        public ExcelTools DefinirWorkSheet(string sheetName)
        {
            if(_workbook is null)
            {
                return this;
            }

            _worksheet = _workbook.Worksheet(sheetName);
            if (_worksheet == null)
                throw new ArgumentException($"La feuille {sheetName} n'existe pas dans le fichier Excel.");

            return this; // Retourne l'instance actuelle pour le chaînage fluent
        }

        // Définir les colonnes par index
        public ExcelTools DefinirColumns(IEnumerable<int> columnsIndexes)
        {
            _columnsIndexes = columnsIndexes.ToList();
            return this; // Retourne l'instance actuelle pour le chaînage fluent
        }

        // Définir les colonnes par noms d'en-tête
        public ExcelTools DefinirColumns(IEnumerable<string> columnsHeaders)
        {
            if (_worksheet is null)
            {
                return this;
            }

            _columnsIndexes = columnsHeaders
                .Select(header => _worksheet.FirstRowUsed().Cells().First(c => c.GetString() == header).Address.ColumnNumber)
                .ToList();

            return this; // Retourne l'instance actuelle pour le chaînage fluent
        }

        public IEnumerable<string> GetAllHeaders() =>
            _worksheet?.FirstRow().Cells()
                .TakeWhile(c => !string.IsNullOrEmpty(c.FormulaR1C1))
                .Select(c => c.FormulaR1C1) ??
            Enumerable.Empty<string>();

        public IEnumerable<IEnumerable<string>> GetFirstLines(int nbLignes) =>
            _worksheet?.Rows().Take(nbLignes)
                        .Select(l => l.Cells()
                        .TakeWhile(c => !string.IsNullOrEmpty(c.FormulaR1C1))
                        .Select(c => c.FormulaR1C1)) ??
            Enumerable.Empty<IEnumerable<string>>();

        public IEnumerable<string> GetHeadersForColumns()
        {
            if (_worksheet is null)
            {
                return Enumerable.Empty<string>();
            }

            // Obtenir toutes les lignes utilisées à partir de la deuxième ligne (ignorer l'en-tête)
            IEnumerable<IXLRow> rows = _worksheet.RowsUsed().Take(1);

            if (rows is null)
            {
                return Enumerable.Empty<string>();
            }

            return GetTextForColumns(rows)?.FirstOrDefault() ?? Enumerable.Empty<string>();
        }

        public IEnumerable<IEnumerable<object>> GetDataForColumns()
        {
            if (_worksheet is null)
            {
                yield break; // Terminer l'itération si la feuille de calcul n'est pas définie
            }

            // Obtenir toutes les lignes utilisées à partir de la deuxième ligne (ignorer l'en-tête)
            IEnumerable<IXLRow>  rows = _worksheet.RowsUsed().Skip(1);

            yield return GetDataForColumns(rows);
        }

        public IEnumerable<IEnumerable<object>> GetDataForColumns(IEnumerable<IXLRow> rows)
        {
            foreach (var row in rows)
            {
                // Pour chaque ligne, obtenir les données des colonnes spécifiées
                IEnumerable<object> rowData = _columnsIndexes
                    .Select(columnIndex => (object)row.Cell(columnIndex).Value); // Sélectionner les valeurs des cellules par index de colonne

                yield return rowData; // Retourne IEnumerable<object> pour chaque ligne
            }
        }

        // Obtenir les données des colonnes en tant qu'IEnumerable<IEnumerable<string>>
        public IEnumerable<IEnumerable<string>> GetTextForColumns()
        {
            return GetDataForColumns()?
                .Select(columnData => columnData?.Select(value => value?.ToString()?? string.Empty)?? Enumerable.Empty<string>()) ??
                Enumerable.Empty<IEnumerable<string>>(); // Fournir une valeur par défaut de type IEnumerable<IEnumerable<string>>
        }

        public IEnumerable<IEnumerable<string>> GetTextForColumns(IEnumerable<IXLRow> rows)
        {
            return GetDataForColumns(rows)?
                .Select(columnData => columnData?.Select(value => value?.ToString() ?? string.Empty) ?? Enumerable.Empty<string>()) ??
                Enumerable.Empty<IEnumerable<string>>(); // Fournir une valeur par défaut de type IEnumerable<IEnumerable<string>>
        }
    }//classe
}//namespace
