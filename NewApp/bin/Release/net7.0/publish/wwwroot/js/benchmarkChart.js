let chart; // Declare chart variable outside the event listener scope

document.addEventListener("DOMContentLoaded", function () {
    const matrixFilterDropdown = document.getElementById("matrixFilter");
    const subAttributeFilterDropdown = document.getElementById("subAttributeFilter"); // New dropdown for subAttribute_new
    const downloadButton = document.getElementById("downloadGraph");
    const disclaimer = document.getElementById("disclaimer");

    fetch('/api/Benchmark/GetAllBenchmarks')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {



            console.log("Data from API:", data);

            // Convert string values to numbers
            data.forEach(item => {
                for (let key in item) {
                    if (!isNaN(parseFloat(item[key]))) {
                        item[key] = parseFloat(item[key]);
                    }
                }
            });

            // Calculate averages for additional attributes
            const attributeAverages = {};
            Object.keys(data[0]).forEach(key => {
                switch (key) {
                    case "twentyTwoToTwentyFour_new":
                        attributeAverages["22-24"] = calculateAverage(data.map(item => item.twentyTwoToTwentyFour_new));
                        break;
                    case "twentyFiveToTwentyNine_new":
                        attributeAverages["25-29"] = calculateAverage(data.map(item => item.twentyFiveToTwentyNine_new));
                        break;
                    case "thirtyToThirtyNine_new":
                        attributeAverages["30-39"] = calculateAverage(data.map(item => item.thirtyToThirtyNine_new));
                        break;
                    case "fortyToFortyNine_new":
                        attributeAverages["40-49"] = calculateAverage(data.map(item => item.fortyToFortyNine_new));
                        break;
                    case "fiftyPlus_new":
                        attributeAverages["50+"] = calculateAverage(data.map(item => item.fiftyPlus_new));
                        break;
                    case "grandTotal_new":
                        attributeAverages["Grand Total"] = calculateAverage(data.map(item => item.grandTotal_new));
                        break;
                    case "lessThanEqual15_new":
                        attributeAverages["<=15"] = calculateAverage(data.map(item => item.lessThanEqual15_new));
                        break;
                    case "sixteenToEighteen_new":
                        attributeAverages["16-18"] = calculateAverage(data.map(item => item.sixteenToEighteen_new));
                        break;
                    case "nineteenToTwentyOne_new":
                        attributeAverages["19-21"] = calculateAverage(data.map(item => item.nineteenToTwentyOne_new));
                        break;
                    default:
                        break;
                }
            });

            console.log("Attribute Averages:", attributeAverages);

            // Define labels in the desired order including x-axis labels
            const labels = ["<=15", "16-18", "19-21", "22-24", "25-29", "30-39", "40-49", "50+", "Grand Total"];

            // Create dataset for the chart
            const distinctColors = [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)',
                // Add more colors as needed
            ];

            // Create dataset for the chart with distinct colors
            const datasets = [{
                label: 'Averages',
                data: labels.map(label => attributeAverages[label]),
                backgroundColor: distinctColors.slice(0, labels.length),
                borderColor: distinctColors.slice(0, labels.length).map(color => color.replace('0.2', '1')),
                borderWidth: 10
            }, {
                label: 'Trend',
                data: labels.map(label => attributeAverages[label]),
                type: 'line',
                fill: false,
                borderColor: 'rgba(255, 0, 0, 1)',
                borderWidth: 2,
                pointRadius: 0,

            }];

            // Adjust font size for the labels directly within the datasets
            datasets.forEach(dataset => {
                dataset.labelFontSize = 20; // Adjust font size for labels
            });

            function isMobileDevice() {
                return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
            }
            var isMobile = isMobileDevice();
            var responsiveOption = true; // Default to true
            if (isMobile) { // Apply fixed dimensions and disable responsiveness only for mobile devices
                console.log("Setting responsiveOption to false for mobile view");
             
                responsiveOption = false
                    ;
        
            }
            if (isMobile) {
                var canvas = document.getElementById("benchmarkChart");
                canvas.setAttribute("width", "800"); // Set width to 1000px for mobile devices
                canvas.setAttribute("height", "800"); // Set height to 1000px for mobile devices
            }
            var titleFontSize = 30; // Default font size for non-mobile devices
            if (isMobile) {
                titleFontSize = 20; // Font size for mobile devices
            }
            var TicksFontSize = 15;
            if (isMobile) {
                TicksFontSize = 10; // Font size for mobile devices
            }


            // Generate the chart
            var ctx = document.getElementById("benchmarkChart").getContext('2d');
            chart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: datasets
                },
                options: {
                    responsive: responsiveOption,
                    plugins: {
                        title: {
                            display: true,
                            color: '#8a2be2',
                            text: 'Employability Score % (India)',
                            position: 'top',
                            align: 'center',
                            font: {
                                weight: 'bold',
                                size: titleFontSize
                            },
                            fullSize: true,
                        },
                    },
                    scales: {
                        y: {
                            beginAtZero: false,
                            suggestedMin: 30,
                            suggestedMax: 60,
                            title: {
                                display: true,
                                align: 'center',
                                text: 'Average Score %',
                                color: 'black',
                                font: {
                                    family: 'Arial',
                                    size: titleFontSize,
                                    weight: 'bold',
                                },
                                padding: {
                                    top: 10,
                                    bottom: 5,
                                    left: 0,
                                    right: 0,
                                },
                            },
                            ticks: {
                                font: {
                                    size: TicksFontSize, // Adjust font size for y-axis labels
                                }
                            }
                        },
                        x: {
                            title: {
                                display: true,
                                align: 'center',
                                text: 'Age Group (Years)',
                                color: 'black',
                                font: {
                                    family: 'Arial',
                                    size: titleFontSize,
                                    weight: 'bold',
                                },
                                padding: {
                                    top: 10,
                                    bottom: 5,
                                    left: 0,
                                    right: 0,
                                },
                            },
                            ticks: {
                                font: {
                                    size: TicksFontSize, // Adjust font size for x-axis labels
                                }
                            }
                        },
                    },
                },
            });




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
            downloadButton.addEventListener("click", function () {
                const canvas = document.getElementById("benchmarkChart");
                const ctx = canvas.getContext('2d');

                // Set font and style for disclaimer text
                ctx.font = "10px Arial"; // Adjust font size and style as needed
                ctx.fillStyle = "red"; // Adjust text color as needed

                // Get the canvas width and height
                const canvasWidth = canvas.width;
                const canvasHeight = canvas.height;

                // Find the Y position for the text relative to average line (estimate based on your image)
                const textY = canvasHeight - 20; // Adjust Y position as needed

                // Set the disclaimer text
                const disclaimerText = "(Below Avg)";

                // Measure the text width to position it horizontally
                const textWidth = ctx.measureText(disclaimerText).width;
                const textX = canvasWidth / 2 - textWidth / 2; // Center the text horizontally

                // Set the canvas background color to white (optional)
                canvas.style.backgroundColor = "white";

                // Draw the text
                ctx.fillText(disclaimerText, textX, textY);

                // Rest of the code to convert canvas to dataURL and download (same as before)
                const image = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");
                const link = document.createElement("a");
                link.setAttribute("href", image);
                link.setAttribute("download", "benchmark_chart.jpg");
                link.click();
            });
        })

        .catch(error => console.error('Error fetching data:', error));
});


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

    // Manually specify the order of labels
    const labels = ["<=15", "16-18", "19-21", "22-24", "25-29", "30-39", "40-49", "50+", "Grand Total"];

    // Define distinct colors for each label
    const labelColors = {
        "<=15": 'rgba(255, 99, 132, 0.2)',
        "16-18": 'rgba(54, 162, 235, 0.2)',
        "19-21": 'rgba(255, 206, 86, 0.2)',
        "22-24": 'rgba(75, 192, 192, 0.2)',
        "25-29": 'rgba(153, 102, 255, 0.2)',
        "30-39": 'rgba(255, 159, 64, 0.2)',
        "40-49": 'rgba(255, 99, 132, 0.2)', // Adjusted alpha value to match the others
        "50+": 'rgba(54, 162, 235, 0.2)',   // Adjusted alpha value to match the others
        "Grand Total": 'rgba(255, 206, 86, 0.2)' // Adjusted alpha value to match the others
    };

    // Create datasets for the chart
    const datasets = [
        {
            label: 'Averages',
            data: labels.map(label => attributeAverages[label]),
            backgroundColor: labels.map(label => labelColors[label]), // Use distinct colors for each label
            borderColor: labels.map(label => labelColors[label].replace('0.2', '1')), // Adjust opacity for borders
            borderWidth: 10
        },
        {
            label: 'Trend',
            data: labels.map(label => attributeAverages[label]), // Use the same data as averages
            type: 'line',
            fill: false, // Don't fill the area under the line
            pointRadius: 0, // Hide data points on the trend line

            // Add a background box around the trend line (without a line)
            backgroundColor: 'rgba(255, 229, 229, 0.2)', // Light red for the box
            borderJoinStyle: 'round', // Round corners for the box
            borderWidth: 1, // Maintain the border for the box
            borderColor: 'rgba(255, 0, 0, 1)', // Use the same red as the box border (optional)
        }
    ];

    // Destroy the existing chart if it exists
    if (chart) {
        chart.destroy();
    }
    function isMobileDevice() {
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    }
    var isMobile = isMobileDevice();
    var responsiveOption = true; // Default to true
    if (isMobile) { // Apply fixed dimensions and disable responsiveness only for mobile devices
        console.log("Setting responsiveOption to false for mobile view");

        responsiveOption = false
            ;

    }
    if (isMobile) {
        var canvas = document.getElementById("benchmarkChart");
        canvas.setAttribute("width", "800"); // Set width to 1000px for mobile devices
        canvas.setAttribute("height", "800"); // Set height to 1000px for mobile devices
    }
    var titleFontSize = 30; // Default font size for non-mobile devices
    if (isMobile) {
        titleFontSize = 20; // Font size for mobile devices
    }
    var TicksFontSize = 15;
    if (isMobile) {
        TicksFontSize = 10; // Font size for mobile devices
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
            responsive: responsiveOption,
            plugins: {
                title: {
                    display: true,
                    color: '#8a2be2',
                    text: 'Employability Score % (India)',
                    position: 'top',
                    align: 'center',
                    font: {
                        weight: 'bold',
                        size: titleFontSize
                    },
                    fullSize: true,
                },
            },
            scales: {
                y: {
                    beginAtZero: false,
                    suggestedMin: 30,
                    suggestedMax: 60,
                    title: {
                        display: true,
                        align: 'center',
                        text: 'Average Score %',
                        color: 'black',
                        font: {
                            family: 'Arial',
                            size: titleFontSize,
                            weight: 'bold',
                        },
                        padding: {
                            top: 10,
                            bottom: 5,
                            left: 0,
                            right: 0,
                        },
                    },
                    ticks: {
                        font: {
                            size: TicksFontSize, // Adjust font size for y-axis labels
                        }
                    }
                },
                x: {
                    title: {
                        display: true,
                        align: 'center',
                        text: 'Age Group (Years)',
                        color: 'black',
                        font: {
                            family: 'Arial',
                            size: titleFontSize,
                            weight: 'bold',
                        },
                        padding: {
                            top: 10,
                            bottom: 5,
                            left: 0,
                            right: 0,
                        },
                    },
                    ticks: {
                        font: {
                            size: TicksFontSize, // Adjust font size for x-axis labels
                        }
                    }
                },
            },
        },
    });
}



// Function to calculate the average of an array of numbers
function calculateAverage(array) {
    const sum = array.reduce((acc, val) => acc + val, 0);
    return sum / array.length;
}
