﻿// Accord Statistics Controls Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Controls
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    using Accord.Statistics.Visualizations;
    using ZedGraph;
    using AForge;

    /// <summary>
    ///   Histogram Box for quickly displaying a form with a histogram 
    ///   on it in the same spirit as System.Windows.Forms.MessageBox.
    /// </summary>
    /// 
    public partial class HistogramBox : Form
    {

        private Thread formThread;

        private HistogramBox()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Blocks the caller until the form is closed.
        /// </summary>
        /// 
        public void WaitForClose()
        {
            if (Thread.CurrentThread != formThread)
                formThread.Join();
        }

        /// <summary>
        ///   Gets the <see cref="HistogramView"/>
        ///   control used to display data in this box.
        /// </summary>
        /// 
        /// <value>The histogram view control.</value>
        /// 
        public HistogramView HistogramView
        {
            get { return histogramView1; }
        }

        /// <summary>
        ///   Sets size of the scatter plot window.
        /// </summary>
        /// 
        /// <param name="width">The desired width.</param>
        /// <param name="height">The desired height.</param>
        /// 
        /// <returns>This instance, for fluent programming.</returns>
        /// 
        public HistogramBox SetSize(int width, int height)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetSize(width, height)));
                return this;
            }

            this.Width = width;
            this.Height = height;

            Refresh();

            return this;
        }

        /// <summary>
        ///   Sets the bins width in the histogram.
        /// </summary>
        /// 
        /// <param name="width">The bin width to be used.</param>
        /// 
        public HistogramBox SetBinWidth(double width)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetBinWidth(width)));
                return this;
            }

            histogramView1.BinWidth = width;
            histogramView1.UpdateGraph();

            Refresh();

            return this;
        }

        /// <summary>
        ///   Sets the number of bins in the histogram.
        /// </summary>
        /// 
        /// <param name="number">The number of bins to be used.</param>
        /// 
        public HistogramBox SetNumberOfBins(int number)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetNumberOfBins(number)));
                return this;
            }

            histogramView1.NumberOfBins = number;
            histogramView1.UpdateGraph();

            Refresh();

            return this;
        }


        /// <summary>
        ///   Displays an histogram with the specified data.
        /// </summary>
        /// 
        /// <param name="values">The histogram values.</param>
        /// 
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static HistogramBox Show(double[] values, bool nonBlocking = false)
        {
            return Show("Histogram", values, nonBlocking);
        }

        /// <summary>
        ///   Displays a histogram with the specified data.
        /// </summary>
        /// 
        /// <param name="title">The title for the histogram window.</param>
        /// <param name="values">The histogram values.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static HistogramBox Show(string title, double[] values, bool nonBlocking = false)
        {
            Histogram histogram = new Histogram(title);
            histogram.Compute(values);
            return show(histogram, nonBlocking);
        }

        /// <summary>
        ///   Displays a histogram.
        /// </summary>
        /// 
        /// <param name="histogram">The histogram to show.</param>
        /// <param name="nonBlocking">If set to <c>true</c>, the caller will continue
        /// executing while the form is shown on screen. If set to <c>false</c>,
        /// the caller will be blocked until the user closes the form. Default
        /// is <c>false</c>.</param>
        /// 
        public static HistogramBox Show(Histogram histogram, bool nonBlocking = false)
        {
            return show(histogram, nonBlocking);
        }


        private static HistogramBox show(Histogram histogram, bool hold)
        {
            HistogramBox form = null;
            Thread formThread = null;

            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);

            formThread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Show control in a form
                form = new HistogramBox();
                form.Text = histogram.Title;
                form.formThread = formThread;
                form.histogramView1.Histogram = histogram;

                stopWaitHandle.Set();

                Application.Run(form);
            });

            formThread.SetApartmentState(ApartmentState.STA);

            formThread.Start();

            stopWaitHandle.WaitOne();

            if (!hold)
                formThread.Join();

            return form;
        }

    }
}
