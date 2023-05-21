let xvalues = [];
let yvalues = [];
for (let i = 0; i < XFields.length; i++) {
    xvalues.push(XFields[i].id);
    yvalues.push(XFields[i].value);
}

const ctx = document.getElementById('myChart');

new Chart(ctx, {
    type: 'bar',
    data: {
        labels: xvalues,//['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
        datasets: [{
            label: '# of Sales',
            data: yvalues,
            borderWidth: 1
        }]
    },
    options: {
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});