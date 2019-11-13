﻿using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A class providing algorithms for principal component analysis
    /// </summary>
    public class PrincipalComponentHelper : MultiVariateAnalysis
    {
        #region Public properties

        /// <summary>
        /// The PCA method
        /// </summary>
        private PrincipalComponentMethod method = PrincipalComponentMethod.Eigendecomposition;
        public PrincipalComponentMethod Method
        {
            get => this.method;
            set
            {
                this.method = value;
                NotifyOfPropertyChange(() => Method);
            }
        }

        /// <summary>
        /// Singular values of the data set
        /// </summary>
        private double[][] eigenVectors;
        public double[][] EigenVectors
        {
            get => this.eigenVectors;
            set
            {
                this.eigenVectors = value;
                NotifyOfPropertyChange(() => EigenVectorsView);
            }
        }

        /// <summary>
        /// The view of the eigen vectors
        /// </summary>
        public DataTable EigenVectorsView
        {
            get
            {
                //Preparing the view
                string[] columnNames = new string[Merge.Columns.Count];
                string[] eigenVectorNames = new string[Merge.Columns.Count];

                for (int i = 0; i < Merge.Columns.Count; i++)
                {
                    columnNames[i] = Merge.Columns[i].ColumnName.ToString();
                    eigenVectorNames[i] = "Eigenvector " + i.ToString();
                }

                DataTable data = EigenVectors.ToTable(eigenVectorNames);

                DataColumn rowNames = new DataColumn("RowNames", typeof(string));

                data.Columns.Add(rowNames.ColumnName, typeof(string));

                try
                { 
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        data.Rows[i][rowNames.ColumnName] = columnNames[i].ToString();
                    }

                }
                catch
                {

                }

                return data;
            }
        }

        /// <summary>
        /// Eigenvalues of the singular values
        /// </summary>
        private double[] eigenValues;
        public double[] EigenValues
        {
            get => this.eigenValues;
            set
            {
                this.eigenValues = value;
                NotifyOfPropertyChange(() => EigenValuesView);
                NotifyOfPropertyChange(() => EigenValuesVarianceView);
            }
        }

        /// <summary>
        /// Eigenvalues of the singular values
        /// </summary>
        private double[] eigenValuesVariance;
        public double[] EigenValuesVariance
        {
            get => this.eigenValuesVariance;
            set
            {
                this.eigenValuesVariance = value;
                NotifyOfPropertyChange(() => EigenValuesVarianceView);
            }
        }

        /// <summary>
        /// Bar chart for the eigenvalues
        /// </summary>
        private BarChartViewModel eigenValueBarChart = new BarChartViewModel()
        {
            Barco = new BarChartObject()
            {
                BarType = BarTypeEnum.Vertical,
                ChartHeight = 200,
                ChartWidth = 200
            }
        };
        public BarChartViewModel EigenValueBarChart
        {
            get => this.eigenValueBarChart;
            set
            {
                this.eigenValueBarChart = value;
                NotifyOfPropertyChange(() => EigenValueBarChart);
            }
        }

        /// <summary>
        /// The view of the eigenvalues
        /// </summary>
        public BindableCollection<double> EigenValuesView
        {
            get
            {
                return new BindableCollection<double>(this.EigenValues.ToList());
            }
        }

        /// <summary>
        /// The view of the eigen vectors
        /// </summary>
        public BindableCollection<double> EigenValuesVarianceView
        {
            get
            {
                return new BindableCollection<double>(this.EigenValuesVariance.ToList());
            }
        }

        /// <summary>
        /// Eigenvalues of the singular values
        /// </summary>
        private double[,] featureVector;
        public double[,] FeatureVector
        {
            get => this.featureVector;
            set
            {
                this.featureVector = value;
                NotifyOfPropertyChange(() => FeatureVector);
            }
        }

        /// <summary>
        /// The projected values
        /// </summary>
        private double[][] projectedValues;
        public double[][] ProjectedValues
        {
            get => this.projectedValues;
            private set
            {
                this.projectedValues = value;
                NotifyOfPropertyChange(() => ProjectedValues);
                NotifyOfPropertyChange(() => ProjectedValuesView);
            }
        }

        /// <summary>
        /// The view of the projected values
        /// </summary>
        public DataTable ProjectedValuesView
        {
            get
            {
                return ProjectedValues.ToTable();
            }
        }

        /// <summary>
        /// Eigenvalues of the singular values
        /// </summary>
        private double[,] covarianceMatrix;
        public double[,] CovarianceMatrix
        {
            get => this.covarianceMatrix;
            set
            {
                this.covarianceMatrix = value;
                NotifyOfPropertyChange(() => CovarianceMatrix);
            }
        }

        /// <summary>
        /// Objects for the scatter chart
        /// </summary>
        private LineAndScatterChartViewModel pc12 = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel PC12
        {
            get => pc12;
            set
            {
                this.pc12 = value;
                NotifyOfPropertyChange(() => PC12);
            }
        }

        /// <summary>
        /// Objects for the scatter chart
        /// </summary>
        private LineAndScatterChartViewModel pc13 = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel PC13
        {
            get => pc13;
            set
            {
                this.pc13 = value;
                NotifyOfPropertyChange(() => PC13);
            }
        }

        /// <summary>
        /// Objects for the scatter chart
        /// </summary>
        private LineAndScatterChartViewModel pc12BiPlot = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel PC12BiPlot
        {
            get => pc12BiPlot;
            set
            {
                this.pc12BiPlot = value;
                NotifyOfPropertyChange(() => PC12BiPlot);
            }
        }

        /// <summary>
        /// Objects for the scatter chart
        /// </summary>
        private LineAndScatterChartViewModel pc13BiPlot = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel PC13BiPlot
        {
            get => pc13BiPlot;
            set
            {
                this.pc13BiPlot = value;
                NotifyOfPropertyChange(() => PC13BiPlot);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dt"></param>
        public PrincipalComponentHelper()
        {

        }

        #endregion

        /// <summary>
        /// Computing the PCA
        /// </summary>
        public async override Task Compute()
        {
            try
            {
                DataTable dat = new DataTable();

                foreach (Mesh dt in DataSet)
                    dat.Merge(dt.Data, true, MissingSchemaAction.Add);

                dat.RemoveNonNumericColumns();
                CollectionHelper.ProcessNumericDataTable(dat);
                dat.RemoveNanRowsAndColumns();
                dat.TreatMissingValues(MissingData);

                Merge = dat;
                //Converting the data set to a matrix
                CalculationDataSet = dat.ToMatrix();

                //Normalizing the dataset
                NormalizeDataSet();

                switch (Method)
                {
                    case PrincipalComponentMethod.Eigendecomposition:

                        // Let's create an analysis with centering (covariance method)
                        // but no standardization (correlation method) and whitening:
                        var pca = new PrincipalComponentAnalysis()
                        {
                            Method = ConvertMethod(AnalysisMethod),
                            Whiten = true
                        };

                        // Now we can learn the linear projection from the data
                        MultivariateLinearRegression transform = pca.Learn(CalculationDataSet.ToJagged());

                        EigenValues = pca.Eigenvalues;
                        EigenValuesVariance = EigenValues.Select(x => x / EigenValues.Sum()).ToArray();
                        EigenVectors = pca.ComponentVectors.Transpose();
                        ProjectedValues = pca.Transform(CalculationDataSet.ToJagged());

                        try
                        {
                            List<Mesh> projectedData = new List<Mesh>();

                            string[] columnNames = new string[Merge.Columns.Count];

                            for (int i = 0; i < Merge.Columns.Count; i++)
                            {
                                columnNames[i] = Merge.Columns[i].ColumnName.ToString();
                            }

                            int j = 0;
                            for (int i = 0; i < DataSet.Count(); i++)
                            {
                                try
                                {
                                    projectedData.Add(new Mesh() { Name = DataSet[i].Name, Data = ProjectedValues.ToTable().AsEnumerable().Skip(j).Take(DataSet[i].Data.Rows.Count).CopyToDataTable() });

                                    for (int k = 0; k < projectedData[i].Data.Columns.Count - 1; k++)
                                        if (k != 0 && k != 2)
                                            projectedData[i].Data.Columns.RemoveAt(k);

                                    j += DataSet[i].Data.Rows.Count;
                                }
                                catch
                                {
                                    continue;
                                }
                            }

                            EigenValueBarChart.Barco.ShallRender = true;

                            List<string> eigenvalues = new List<string>();
                            List<string> eigenvectors = new List<string>();

                            for (int i = 0; i < EigenValues.Count(); i++)
                                eigenvalues.Add("Eigenvalue " + i);

                            EigenValueBarChart.Barco.XLabels.Clear();

                            foreach (string eig in eigenvalues)
                                EigenValueBarChart.Barco.XLabels.Add(new Label() { Text = eig });

                            ///Creating eigen value bar chart
                            EigenValueBarChart.Barco.YLabel.Text = "Loading";
                            EigenValueBarChart.Barco.Ymax = EigenValues.Max() + 2;
                            EigenValueBarChart.Barco.Xmax = EigenValues.Count() + 1;
                            EigenValueBarChart.Barco.CreateCategoricHistogram(eigenvalues.ToArray(), EigenValues);
                            EigenValueBarChart.Barco.XLabel.Text = "Eigenvalue#";
                            EigenValueBarChart.Barco.YLabel.Text = "Eigenvalue";

                            //Creating visualization of the projected values
                            PC12.Lco.ShallRender = true;
                            PC12.Lco.XLabel.Text = "PC1";
                            PC12.Lco.YLabel.Text = "PC2";
                            PC12.Lco.DataSet = new BindableCollection<Mesh>(projectedData);
                            PC12.Lco.CreateLineChart();

                            //Creating visualization of the projected values
                            PC13.Lco.ShallRender = true;
                            PC13.Lco.Direction = DirectionEnum.XZ;
                            PC13.Lco.XLabel.Text = "PC1";
                            PC13.Lco.YLabel.Text = "PC3";
                            PC13.Lco.DataSet = new BindableCollection<Mesh>(projectedData);
                            PC13.Lco.CreateLineChart();

                            foreach (DataColumn column in dat.Columns)
                            {
                                eigenvectors.Add(column.ColumnName);
                            }

                            //Creating visualization of the eigenvectors
                            PC12BiPlot.Lco.ShallRender = true;
                            PC12BiPlot.Lco.XLabel.Text = "PC1";
                            PC12BiPlot.Lco.YLabel.Text = "PC2";
                            PC12BiPlot.Lco.Direction = DirectionEnum.X;
                            PC12BiPlot.Lco.DataSet.Clear();

                            //Creating visualization of the projected values
                            PC13BiPlot.Lco.ShallRender = true;
                            PC13BiPlot.Lco.DataSet.Clear();
                            PC13BiPlot.Lco.Direction = DirectionEnum.Y;
                            PC13BiPlot.Lco.XLabel.Text = "PC1";
                            PC13BiPlot.Lco.YLabel.Text = "PC3";

                            //Adding a data series for each biplot member
                            for (int i = 1; i < EigenVectorsView.Rows.Count; i++)
                            {
                                Mesh biplot = new Mesh() { Name = columnNames[i] };
                                DataTable dt = EigenVectorsView.Clone();
                                dt.Rows.Add(EigenVectorsView.Rows[i].ItemArray);

                                for (int f = 0; f < dt.Columns.Count; f++)
                                    dt.Rows[0][f] = 0;

                                dt.Rows.Add(EigenVectorsView.Rows[i].ItemArray);

                                biplot.Data = dt;

                                PC12BiPlot.Lco.DataSet.Add(biplot);
                                PC13BiPlot.Lco.DataSet.Add(biplot);
                            }

                            PC12BiPlot.Lco.CreateLineChart();
                            PC13BiPlot.Lco.CreateLineChart();

                            for(int i = 0; i< PC12BiPlot.Lco.DataCollection.Count(); i++)
                            {
                                PC12BiPlot.Lco.DataCollection[i].Symbols.FillColor = ColorHelper.PickBrush();
                                PC13BiPlot.Lco.DataCollection[i].Symbols.FillColor = ColorHelper.PickBrush();
                            }
                        }
                        catch
                        {
                            return;
                        }
                        break;
                }

            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Converting a Accord.Statistics.Analysis.AnalysisMethod to a Accord.Statistics.Analysis.PrincipalComponentMethod
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public Accord.Statistics.Analysis.PrincipalComponentMethod ConvertMethod(Accord.Statistics.Analysis.AnalysisMethod method)
        {
            switch (method)
            {
                case AnalysisMethod.Center:
                    return Accord.Statistics.Analysis.PrincipalComponentMethod.Center;
                case AnalysisMethod.Standardize:
                    return Accord.Statistics.Analysis.PrincipalComponentMethod.Standardize;
                default:
                    return Accord.Statistics.Analysis.PrincipalComponentMethod.Center;
            }
        }
    }
}