$(function () {

  // #region Modal

  $('#ModalCenter').modal('show');

  // #endregion

  // #region NAV Chart.js

  //data for NAV charts
  if (window.navsJson != null) {

    const navs = JSON.parse(window.navsJson.replace(/&quot;/g, '"'));

    const weekValues = [];
    const issuanceNavs = [];
    const cancellationNavs = [];
    const statisticalNavs = [];

    navs.forEach(e => {
      weekValues.push(e["Date"]);
      issuanceNavs.push(e["Issuance"]);
      cancellationNavs.push(e["Cancellation"]);
      statisticalNavs.push(e["Statistical"]);
    });

    const weekDatasets = [
      {
        label: "صدور",
        data: issuanceNavs,
        backgroundColor: "#095c5c",
        borderColor: "#095c5c",
        pointStyle: true,
        pointRadius: 0.5,
        pointHoverRadius: 0.5,
        pointHoverBackgroundColor: "#595d5f60",
        fill: false,
        cubicInterpolationMode: 'monotone',
        tension: 0.4,
      },
      {
        label: "ابطال",
        data: cancellationNavs,
        backgroundColor: "#E8885E",
        borderColor: "#E8885E",
        pointStyle: true,
        pointRadius: 0.5,
        pointHoverRadius: 0.5,
        pointHoverBackgroundColor: "#595d5f60",
        fill: false,
        cubicInterpolationMode: 'monotone',
        tension: 0.4,
      },
      {
        label: "آماری",
        data: statisticalNavs,
        borderColor: "#79b2ba",
        backgroundColor: "#79b2ba",
        pointStyle: true,
        pointRadius: 0.5,
        pointHoverRadius: 0.5,
        pointHoverBackgroundColor: "#595d5f60",
        fill: false,
        cubicInterpolationMode: 'monotone',
        tension: 0.4,
      }
    ];

    // Chart Initialization Function
    Chart.defaults.font.family = "IRANSansFarsi";
    Chart.defaults.font.weight = "bold";
    function initializeLineChart(chartId, labels, datasets) {
      new Chart(chartId, {
        type: 'line',
        data: {
          labels: labels,
          datasets: datasets,
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          interaction: {
            intersect: true,
            mode: 'x'
          },
          plugins: {
            title: {
              display: false,
            },
            tooltip: {
              rtl: true,
              padding: 10,
              usePointStyle: true,
              bodySpacing: 10,
              titleMarginBottom: 15,
              boxPadding: 5,
            },
          },
          scales: {
            x: {
              display: true,
              ticks: {
                minRotation: 0,
                autoskip: true,
                autoSkipPadding: 50,
              },
            },
            y: {
              display: true,
              suggestedMin: 400,
              suggestedMax: 500,
            },
          },
        },
      });
    }

    if ($("#navs-chart").length > 0) {
      initializeLineChart("navs-chart", weekValues, weekDatasets);
    }
  }

  // #endregion

  // #region Asset Composition Doughnut Charts

  //Asset composition chart
  if (window.assetCompositionChartDataJson != null) {
    const assetCompositionData = JSON.parse(window.assetCompositionChartDataJson.replace(/&quot;/g, '"'));

    const assetCompositionChartData = {
      labels: Object.keys(assetCompositionData),
      datasets: [
        {
          data: Object.values(assetCompositionData),
          backgroundColor: [
            '#045050',
            '#E8885E',
            '#B5D6DB',
            "#EAE2D5",
            '#E8885E'
          ],
          hoverBackgroundColor: [
            '#045050',
            '#E8885E',
            '#B5D6DB',
            "#EAE2D5",
            '#E8885E'
          ],
          hoverOffset: 0,
          borderWidth: 4,
        }
      ]
    };

    if ($("#asset-composition-chart").length > 0) {
      initializeDoughnutChart("asset-composition-chart", assetCompositionChartData);
    }
  }

  // Sectors assets composition chart
  if (window.sectorsAssetsCompositionData != null) {
    const sectorsAssetsCompositionData = JSON.parse(window.sectorsAssetsCompositionData.replace(/&quot;/g, '"'));

    const sectorsAssetsCompositionChartData = {
      labels: Object.keys(sectorsAssetsCompositionData),
      datasets: [
        {
          data: Object.values(sectorsAssetsCompositionData),
          backgroundColor: [
            '#045050',
            '#E8885E',
            '#B5D6DB',
            "#EAE2D5",
            '#E8885E'
          ],
          hoverBackgroundColor: [
            '#045050',
            '#E8885E',
            '#B5D6DB',
            "#EAE2D5",
            '#E8885E'
          ],
          hoverOffset: 0,
          borderWidth: 4,
        }
      ]
    };

    if ($("#industry-composition-chart").length > 0) {
      initializeDoughnutChart("industry-composition-chart", sectorsAssetsCompositionChartData);
    }
  }

  // #endregion
});

// #region Initialize Doughnut Charts

//Overlapping rounded segments plugin
const overlappingSegments = {
  id: 'overlappingSegments',
  afterDatasetDraw(chart) {
    const { ctx, data } = chart;
    const x = chart.getDatasetMeta(0).data[0].x;
    const y = chart.getDatasetMeta(0).data[0].y;
    const angle = Math.PI / 180;
    const dataLength = chart.getDatasetMeta(0).data.length;

    for (let index = dataLength - 1; index >= 0; index--) {
      const innerRadius = chart.getDatasetMeta(0).data[index].innerRadius;
      const outerRadius = chart.getDatasetMeta(0).data[index].outerRadius;
      const endAngle = chart.getDatasetMeta(0).data[index].endAngle;
      const radius = (outerRadius - innerRadius) / 2;
      const coordinates = [];
      const xCoor = (innerRadius + radius) * Math.cos(endAngle + Math.PI);
      const yCoor = (innerRadius + radius) * Math.sin(endAngle);
      coordinates.push({ x: xCoor, y: yCoor });
      ctx.save();
      ctx.fillStyle = data.datasets[0].backgroundColor[index];
      ctx.translate(x, y);
      ctx.beginPath();
      coordinates.forEach(({ x, y }) => {
        ctx.arc(-x, y, radius - 1, 0, angle * 360, false);
      });
      ctx.fill();
      ctx.restore();
    };
  }
}

//Centralize label on hover
const centeralizeHoverLabel = {
  id: 'centeralizeHoverLabel',
  afterDraw(chart) {
    const { ctx, chartArea: { left, top, width, height } } = chart;
    ctx.save();

    ctx.font = 'bold 20px IRANSansFarsi';
    ctx.fillStyle = '#494e4e';
    ctx.textAlign = 'center';

    if (chart._active.length > 0) {
      const label = chart.config.data.labels[chart._active[0].index];
      const value = chart.config.data.datasets[chart._active[0].datasetIndex].data[chart._active[0].index];
      ctx.fillText(`${label}`, width / 2 + left, height / 2 + top - 10);
      ctx.font = 'bolder 24px IRANSansFarsi';
      ctx.fillText(`${value}%`, width / 2 + left, height / 2 + top + 25);
    }
    else ctx.fillText(`خالص ارزش دارایی‌ها`, width / 2 + left, height / 2 + top);
  }
}

// Asset Composition Chart Initialization
function initializeDoughnutChart(chartId, chartData) {

  // Function to create custom labels
  function generateCustomLegendLabels() {
    const originalLabels = chartData.labels;
    const customLabels = originalLabels.map((label, index) => {
      const dataset = chartData.datasets[0];
      const value = dataset.data[index];
      return {
        ...label,
        text: `${label} : %${value.toFixed(2)}`,
        fillStyle: dataset.backgroundColor[index],
        strokeStyle: 'rgba(0,0,0,0)',
      };
    });
    return customLabels;
  }

  new Chart(chartId, {
    type: 'doughnut',
    data: chartData,
    plugins: [overlappingSegments, centeralizeHoverLabel],
    options: {
      responsive: true,
      cutout: function () {
        return $(window).width() < 1201 ? 40 : 130;
      },
      plugins: {
        legend: {
          position: 'left',
          rtl: true,
          labels: {
            usePointStyle: true,
            pointStyleWidth: 20,
            boxHeight: 20,
            generateLabels: generateCustomLegendLabels,
          }
        },
        title: {
          display: false,
        },
        tooltip: {
          enabled: false
        }
      },
    },
  });
}

// #endregion

// #region Calculater

const startDateInput = document.querySelector("#startDate");
const finishDateInput = document.querySelector("#finishDate");
const totalFinalAmount = document.querySelector("#total-final-amount");
const totalFinalProfitAmount = document.querySelector("#total-final-profit-amount");
const totalFinalProfitPercent = document.querySelector("#total-final-profit-percent");

$("#investmentAmount").on("click input", updateCalculator);
$("#startDate").on("change", updateCalculator);
$("#finishDate").on("change", updateCalculator);

// Function to update calculator values
function updateCalculator() {

  const inputValue = investmentAmountInput.value.trim();
  const investmentAmount = inputValue === '' ? 0 : parseFloat(inputValue.replace(/,/g, ''));
  const formattedInvestmentAmount = investmentAmount.toLocaleString();

  const startDate = new Date(startDateInput.value);
  const finishDate = new Date(finishDateInput.value);

  const initialUnits = investmentAmount / getIssuanceNavPrice(startDate);
  const finalValue = initialUnits * getCancellationNavPrice(finishDate);
  const profit = finalValue - investmentAmount;
  const profitPercent = investmentAmount == 0 ? 0 : (profit / investmentAmount) * 100;

  totalFinalAmount.innerHTML = `<span>${finalValue.toLocaleString()}</span> تومان`;
  totalFinalProfitAmount.textContent = profit.toLocaleString() + " تومان";
  totalFinalProfitPercent.textContent = "%" + profitPercent.toFixed(2);

  // Update input field with formatted value
  investmentAmountInput.value = formattedInvestmentAmount;
}

// Clear input value when clicked
function clearInputValue() {
  investmentAmountInput.value = '';
}

function getIssuanceNavPrice(date) {
  return 100; // Example value
}

function getCancellationNavPrice(date) {
  return 190; // Example value
}

// #endregion
