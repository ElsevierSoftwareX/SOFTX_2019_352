﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System;
using MoreLinq;
using Rh.DateRange.Picker.Common;
using System.IO;
using Microsoft.Win32;

namespace GeoReVi
{
    /// <summary>
    /// View model to load, display and transform data sets from the database
    /// </summary>
    public class MultivariateDataViewModel : PropertyChangedBase
    {

        #region Public Properties

        /// <summary>
        /// Columns available in the data set
        /// </summary>
        private ObservableCollection<string> dataTableColumnNames = new ObservableCollection<string>();
        public ObservableCollection<string> DataTableColumnNames
        {
            get => this.dataTableColumnNames;
            set
            {
                this.dataTableColumnNames = value;
                NotifyOfPropertyChange(() => DataTableColumnNames);
            }
        }

        /// <summary>
        /// Columns available in the data set
        /// </summary>
        private ObservableCollection<string> dataCatergoricTableColumnNames = new ObservableCollection<string>();
        public ObservableCollection<string> DataCatergoricTableColumnNames
        {
            get => this.dataTableColumnNames;
            set
            {
                this.dataCatergoricTableColumnNames = value;
                NotifyOfPropertyChange(() => DataCatergoricTableColumnNames);
            }
        }

        /// <summary>
        /// Selected column that should be processed
        /// </summary>
        private ObservableCollection<string> selectedColumn = new ObservableCollection<string>();
        public ObservableCollection<string> SelectedColumn
        {
            get => this.selectedColumn;
            set
            {
                this.selectedColumn = value;
                NotifyOfPropertyChange(() => SelectedColumn);
            }
        }

        /// <summary>
        /// Selected categoric column that should be processed
        /// </summary>
        private string selectedCategoricColumn = "";
        public string SelectedCategoricColumn
        {
            get => this.selectedCategoricColumn;
            set
            {
                this.selectedCategoricColumn = value;
                NotifyOfPropertyChange(() => SelectedCategoricColumn);
            }
        }

        /// <summary>
        /// Measurement points of a property
        /// </summary>
        private BindableCollection<Mesh> measPoints = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> MeasPoints
        {
            get => this.measPoints;
            set
            {
                this.measPoints = value;
                NotifyOfPropertyChange(() => MeasPoints);
            }
        }

        /// <summary>
        /// The selected Item
        /// </summary>
        private Mesh selectedMeasPoint = new Mesh();
        public Mesh SelectedMeasPoint
        {
            get => this.selectedMeasPoint;
            set
            {
                this.selectedMeasPoint = value;
                NotifyOfPropertyChange(() => SelectedMeasPoint);
            }
        }

        //variable to check to show all objects
        private bool all;
        public bool All
        {
            get => this.all;
            set
            {
                this.all = value;
                NotifyOfPropertyChange(() => All);
            }
        }

        /// <summary>
        /// Variable the data set will be grouped by
        /// </summary>
        private string groupBy;
        public string GroupBy
        {
            get => this.groupBy;
            set
            {
                this.groupBy = value;
                NotifyOfPropertyChange(() => GroupBy);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_parameterClass"></param>
        public MultivariateDataViewModel()
        {

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Removing a column from the meas points data table
        /// </summary>
        public void RemoveColumn()
        {
            try
            {
                try
                {
                    SelectedMeasPoint.Data.AcceptChanges();
                }
                catch
                {

                }

                foreach (Mesh dt in MeasPoints)
                {
                    try
                    {
                        dt.Data.Columns.Remove(SelectedColumn[0]);
                    }
                    catch
                    {
                        continue;
                    }
                }

                SetDataTableNames();
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(-1));
            }
        }

        /// <summary>
        /// Grouping task
        /// </summary>
        public async Task GroupBySelectedColumn()
        {
            try
            {
                try
                {
                    SelectedMeasPoint.Data.AcceptChanges();
                }
                catch
                {

                }

                List<Mesh> groupedMeshes = new List<Mesh>();

                List<string> names = SelectedMeasPoint.Data.AsEnumerable().Select(x => x.Field<string>(SelectedCategoricColumn)).Distinct().ToList();

                names.ForEach(x =>
                {
                    Mesh mesh = new Mesh();
                    mesh.Data = SelectedMeasPoint.Data.AsEnumerable().Where(y => y.Field<string>(SelectedCategoricColumn) == x).CopyToDataTable();
                    mesh.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(SelectedMeasPoint.Vertices.Where(y => y.Name == x).ToList());
                    mesh.Name = x;
                    groupedMeshes.Add(mesh);
                });

                MeasPoints.AddRange(groupedMeshes);
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(-1));
            }
        }

        /// <summary>
        /// Removing a data set from the data collection based on the key
        /// </summary>
        /// <param name="key">Key of the data set</param>
        public void RemoveDataSet()
        {
            try
            {
                MeasPoints.Remove(SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Removing a column from the meas points data table
        /// </summary>
        public void RemoveColumn(string columnName)
        {
            try
            {
                foreach (Mesh dt in MeasPoints)
                {
                    dt.Data.Columns.Remove(CollectionHelper.GetPropertyColumnName(columnName));
                }

                SetDataTableNames();
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
            }
        }

        /// <summary>
        /// Updates the dataSet
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="newKey"></param>
        public void UpdateHeader(string newKey)
        {
            try
            {
                SelectedMeasPoint.Name = newKey;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Importing a dropped object of investigation data file
        /// </summary>
        /// <param name="e"></param>
        public void ImportMesh()
        {
            try
            {
                //File dialog for opening a jpeg, png or bmp file
                OpenFileDialog openFileDlg = new OpenFileDialog();
                openFileDlg.Filter = @"Excel (*.xlsx;*.xls)|*.xlsx;*.xls|CSV (*.csv)|*.csv|GeoReVi Mesh (*.gmsh)|*.gmsh";
                openFileDlg.RestoreDirectory = true;
                openFileDlg.ShowDialog();

                if (openFileDlg.FileName == "")
                {
                    return;
                }

                //Getting file information
                FileInfo fi = new FileInfo(openFileDlg.FileName);

                DataTable table = new DataTable() { TableName = "MyTableName" };

                if (fi.Extension == ".XLSX" || fi.Extension == ".xlsx")
                {
                    DataSet tables = FileHelper.LoadWorksheetsInDataSheets(fi.FullName, false, "", fi.Extension);
                    table = tables.Tables[0];
                }
                else if (fi.Extension == ".CSV" || fi.Extension == ".csv")
                {
                    table = FileHelper.CsvToDataTable(fi.FullName, true);
                }
                else if (fi.Extension == ".gmsh" || fi.Extension == ".gmsh")
                {
                    MeasPoints.Add((Mesh)fi.FullName.FromXml<Mesh>());
                }

                MeasPoints.Add(new Mesh() { Name = "New data set", Data = table });
                SetDataTableNames();
            }
            catch (Exception ex)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(ex);
            }
        }



        /// <summary>
        /// returning the header names of the MultiParameterViewModel.MeasPoints data table
        /// </summary>
        public async Task SetDataTableNames()
        {
            try
            {

                BindableCollection<string> columnNames = new BindableCollection<string>();
                BindableCollection<string> columnCategoricNames = new BindableCollection<string>();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    DataTableColumnNames.Clear();
                });

                DataCatergoricTableColumnNames.Clear();
                DataTableColumnNames.Clear();

                for (int i = 0; i < MeasPoints.Count(); i++)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        DataTable dt = MeasPoints[i].Data.Clone();

                        //Getting all categoric column names
                        columnCategoricNames = new BindableCollection<string>(dt.Columns.Cast<DataColumn>().Where(x => x.DataType == typeof(string))
                                                        .Select(x => x.ColumnName).ToList());

                        dt.RemoveNonNumericColumns();

                        if (MeasPoints.Count > 0)
                        {
                            //Getting all numeric column names
                            columnNames = new BindableCollection<string>(dt.Columns.Cast<DataColumn>()
                                     .Select(x => x.ColumnName).ToList());

                            //Adding all numeric column names to the collection
                            foreach (string columnName in columnNames)
                                if (!DataTableColumnNames.Contains(columnName))
                                    DataTableColumnNames.Add(columnName);

                            //Adding all categoric column names to the collection
                            foreach (string columnCategoricName in columnCategoricNames)
                                if (!DataCatergoricTableColumnNames.Contains(columnCategoricName))
                                    DataCatergoricTableColumnNames.Add(columnCategoricName);

                        }

                        SelectedColumn.Clear();

                        SelectedColumn.Add(DataTableColumnNames[0]);
                        SelectedColumn.Add(DataTableColumnNames[1]);
                        SelectedColumn.Add(DataTableColumnNames[2]);

                        SelectedCategoricColumn = DataCatergoricTableColumnNames.FirstOrDefault();
                    });
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Applies a logarithmic transformation on the selected column
        /// </summary>
        /// <param name="columnName"></param>
        public void LogarithmicTransformation()
        {
            //Checking for negative values
            MeasPoints.ForEach(x =>
            {
                if (x.Data.AsEnumerable().Where(y => y.Field<double>(SelectedColumn[0]) <= 0).Count() > 0)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("Cannot log-transform negative values");
                    return;
                }
            });

            //Transforming data set
            foreach (Mesh dt in MeasPoints)
            {
                for (int i = 0; i < dt.Data.Rows.Count; i++)
                {
                    dt.Data.Rows[i].SetField<double>(SelectedColumn[0], Math.Log10((double)dt.Data.Rows[i].Field<double>(SelectedColumn[0])));
                }
            }
        }

        /// <summary>
        /// Applying a z transformation on the dataset
        /// </summary>
        public void ZScoreTransformation()
        {
            //Transforming data set
            foreach (Mesh dt in MeasPoints)
            {
                int count = dt.Data.Rows.Count;
                double avg = dt.Data.AsEnumerable().Average(r => r.Field<double>(0));
                double sum = dt.Data.AsEnumerable().Sum(r => (r.Field<double>(0) - avg) * (r.Field<double>(0) - avg));

                for (int i = 0; i < dt.Data.Rows.Count; i++)
                {
                    dt.Data.Rows[i].SetField<double>(SelectedColumn[0], dt.Data.Rows[i].Field<double>(SelectedColumn[0]) - Math.Sqrt(sum / Convert.ToDouble(count)));
                }
            }
        }

        #endregion
    }
}