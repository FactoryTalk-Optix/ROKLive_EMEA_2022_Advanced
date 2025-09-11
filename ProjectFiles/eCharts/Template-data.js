var dom = document.getElementById('container');
var myChart = echarts.init(dom, 'dark', {
    renderer: 'svg',
    useDirtyRect: false
});
var app = {};

var option;

option = $DATA$;

if (option && typeof option === 'object') {
    myChart.setOption(option);
}

// Add auto-update functionality
window.updateChartData = function(newValues) {
    if (myChart) {
        var currentOption = myChart.getOption();
        // Update only the values in the series data
        currentOption.series[0].data[0].value = newValues;
        // Apply the updated option
        myChart.setOption(currentOption, false); // false = merge instead of replace
        console.log('Chart updated with new values:', newValues);
        return true;
    }
    return false;
};

// Check for updates periodically
function checkForUpdates() {
    // Create a fetch request to get the latest data
    fetch('./data.json?' + new Date().getTime()) // Add timestamp to prevent caching
        .then(response => response.json())
        .then(data => {
            if (data && data.series && data.series[0] && data.series[0].data && data.series[0].data[0]) {
                window.updateChartData(data.series[0].data[0].value);
            }
        })
        .catch(error => console.error('Error fetching update data:', error));
}

// Start the update cycle
setInterval(checkForUpdates, 1000);

window.addEventListener('resize', myChart.resize);