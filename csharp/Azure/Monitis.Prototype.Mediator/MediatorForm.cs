using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Monitis.Prototype.Logic.Azure;
using Monitis.Prototype.Logic.Azure.TableService;
using Monitis.Prototype.Logic.Common;
using Monitis.Prototype.Logic.Mediation;

namespace Monitis.Prototype.UI
{
    /// <summary>
    /// Class represents form for mediation process between Azure and Monitis
    /// </summary>
    public partial class MediatorForm : Form
    {
        /// <summary>
        /// User session object
        /// </summary>
        private readonly UserSession _userSession;

        /// <summary>
        /// Constructor form
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="mdiParent"></param>
        public MediatorForm(UserSession userSession, Form mdiParent)
        {
            InitializeComponent();
            lblStatus.Text = String.Empty;
            if (userSession == null)
            {
                throw new ArgumentNullException("userSession");
            }
            MdiParent = mdiParent;
            _userSession = userSession;

            chartControl.ChartAreas["CPUArea"].AxisY.Minimum = 0;
            chartControl.ChartAreas["CPUArea"].AxisY.Maximum = 100;
            btnStop.Enabled = false;
        }

        /// <summary>
        /// Handler for start mediation process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartClick(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            //create new mediator object and schedule mediation with timer
            _mediator = new Mediator(new TimeSpan(0, 0, 0, (int)nupField.Value), _userSession);
            _mediator.CPUDataUpdated += OnCPUDataUpdated;
            _mediator.MemoryUpdated += OnMemoryUpdated;
            _mediator.StatusChanged += OnMediatorStatusChanged;
            _mediator.Start();
        }

        /// <summary>
        /// Handler for mediation process status changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMediatorStatusChanged(object sender, StatusEventArgs e)
        {
            lblStatus.UIThread(() =>
                              {
                                  lblStatus.Text = @"What it is going on:  " + e.Status;
                              });

        }

        /// <summary>
        /// Handler for memory counter. Render new perfomance data to chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMemoryUpdated(object sender, CounterDataEventArgs e)
        {
            if (e.PerformanceDatas != null)
            {
                //get series for memory
                Series series = chartControl.Series[1];
                chartControl.UIThread(() =>
                {
                    series.Points.Clear();
                    foreach (PerformanceData point in e.PerformanceDatas)
                    {
                        series.Points.AddXY(new DateTime(point.EventTickCount), point.CounterValue);
                    }
                });
            }
        }

        /// <summary>
        /// Handler for CPU counter. Render new perfomance data to chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCPUDataUpdated(object sender, CounterDataEventArgs e)
        {
            if (e.PerformanceDatas != null)
            {
                //get series for CPU
                Series series = chartControl.Series[0];
                chartControl.UIThread(() =>
                                    {
                                        //series.Points.Clear();
                                        foreach (PerformanceData point in e.PerformanceDatas)
                                        {

                                            series.Points.AddXY(new DateTime(point.EventTickCount), point.CounterValue);
                                        }
                                    });

            }
        }

        /// <summary>
        /// Handler for stop mediation process event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStopClick(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            if (_mediator != null)
            {
                _mediator.Stop();
            }
        }

        private Mediator _mediator;
    }
}