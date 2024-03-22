let chart; // Declare chart variable outside the event listener scope

document.addEventListener("DOMContentLoaded", function () {
    const matrixFilterDropdown = document.getElementById("matrixFilter");
    const subAttributeFilterDropdown = document.getElementById("subAttributeFilter"); // New dropdown for subAttribute_new

    document.getElementById("showGraphButton").addEventListener("click", function () {
        fetch('/api/Benchmark/GetAllBenchmarks')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log("Data from API:", data);

                // Extract unique matrix values
                const uniqueMatrixValues = [...new Set(data.map(item => item.matrix_new))];
                const uniqueSubAttributeValues = [...new Set(data.map(item => item.subAttribute_new))]; // Unique subAttribute_new values

                console.log("Unique Matrix Values:", uniqueMatrixValues);
                console.log("Unique SubAttribute Values:", uniqueSubAttributeValues);

                // Populate the matrix filter dropdown
                uniqueMatrixValues.forEach(value => {
                    const option = document.createElement("option");
                    option.text = value;
                    option.value = value;
                    matrixFilterDropdown.add(option);
                });

                // Populate the subAttribute filter dropdown
                uniqueSubAttributeValues.forEach(value => {
                    const option = document.createElement("option");
                    option.text = value;
                    option.value = value;
                    subAttributeFilterDropdown.add(option);
                });

                // Event listener for matrix filter dropdown
                matrixFilterDropdown.addEventListener("change", function () {
                    console.log("Selected Matrix Value:", matrixFilterDropdown.value);
                    updateChart(matrixFilterDropdown.value, data);
                });

                // Event listener for subAttribute filter dropdown
                subAttributeFilterDropdown.addEventListener("change", function () {
                    console.log("Selected SubAttribute Value:", subAttributeFilterDropdown.value);
                    updateChart(subAttributeFilterDropdown.value, data);
                });
            })
            .catch(error => console.error('Error fetching data:', error));
    });
});

// Function to update the chart based on selected value
// Function to update the chart based on selected value
function updateChart(selectedValue, data) {
    const selectedData = data.filter(item => item.matrix_new === selectedValue || item.subAttribute_new === selectedValue);

    console.log("Selected Data:", selectedData);

    // Calculate averages for the selected attributes
    const attributeAverages = {};
    Object.keys(selectedData[0]).forEach(key => {
        if (["lessThanEqual15_new", "sixteenToEighteen_new", "nineteenToTwentyOne_new", "twentyTwoToTwentyFour_new", "twentyFiveToTwentyNine_new", "thirtyToThirtyNine_new", "fortyToFortyNine_new", "fiftyPlus_new", "grandTotal_new"].includes(key)) {
            let label;
            switch (key) {
                case "lessThanEqual15_new":
                    label = "<=15";
                    break;
                case "sixteenToEighteen_new":
                    label = "16-18";
                    break;
                case "nineteenToTwentyOne_new":
                    label = "19-21";
                    break;
                case "twentyTwoToTwentyFour_new":
                    label = "22-24";
                    break;
                case "twentyFiveToTwentyNine_new":
                    label = "25-29";
                    break;
                case "thirtyToThirtyNine_new":
                    label = "30-39";
                    break;
                case "fortyToFortyNine_new":
                    label = "40-49";
                    break;
                case "fiftyPlus_new":
                    label = "50+";
                    break;
                case "grandTotal_new":
                    label = "Grand Total";
                    break;
                default:
                    label = key; // Use key as label if no specific format is defined
            }
            attributeAverages[label] = calculateAverage(selectedData.map(item => parseFloat(item[key])));
        }
    });

    console.log("Attribute Averages:", attributeAverages);

    // Create labels for the chart
    const labels = Object.keys(attributeAverages);

    // Create dataset for the chart
    const datasets = [{
        label: 'Averages',
        data: Object.values(attributeAverages),
        backgroundColor: 'rgba(255, 99, 132, 0.2)',
        borderColor: 'rgba(255, 99, 132, 1)',
        borderWidth: 1
    }];

    // Destroy the existing chart if it exists
    if (chart) {
        chart.destroy();
    }

    // Create the new chart
    var ctx = document.getElementById("benchmarkChart").getContext('2d');
    chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        
                    }
                }]
            }
        }
    });
}


// Function to calculate the average of an array of numbers
function calculateAverage(array) {
    const sum = array.reduce((acc, val) => acc + val, 0);
    return sum / array.length;
}
