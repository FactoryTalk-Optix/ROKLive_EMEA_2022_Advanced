var dom = document.getElementById('container');
        var myChart = echarts.init(dom, 'dark', {
            renderer: 'svg',
            useDirtyRect: false
        });
        var app = {};

        var option;

        option = {
            /*title: {
                text: 'Radar Chart: Energy consumption',
                textStyle: {
                    fontSize: 30,
                    fontFamily: "Share Tech Mono",
                    fontWeight: "normal"
                },
            },
            legend: {
                data: ['Max Value', 'Actual value']
            },*/
			//backgroundColor: "";
            radar: {
                shape: 'circle',
                silent: true,
                indicator: [
                    {
                        name: 'Main Thrusters',
                        max: 20000,
                        color: "#ffffff"
                    },
                    {
                        name: 'Lateral Thrusters',
                        max: 20000,
                        color: "#ffffff"
                    },
                    {
                        name: 'Oxygen',
                        max: 20000,
                        color: "#ffffff"
                    },
                    {
                        name: 'Water Treatment',
                        max: 20000,
                        color: "#ffffff"
                    },
                    {
                        name: 'Drilling',
                        max: 20000,
                        color: "#ffffff"
                    },
                    {
                        name: 'Stabilization',
                        max: 20000,
                        color: "#ffffff"
                    }
                ],
                axisName: {
                    fontSize: 15,
                    fontFamily: "Share Tech Mono"
                },
                axisLine: {
                    lineStyle: {
                        color: "#375454",
                        width: 2,
                        cap: "round"
                    },
                }
            },
            series: [
                {
                    name: 'Energy consumption values',
                    type: 'radar',
                    data: [
                        {
                            value: [$01, $02, $03, $04, $05, $06],
                            name: 'Line 1'
                        },
                        /*{
                            value: [$07, $08, $09, $10, $11, $12],
                            name: 'Line 2'
                        }*/
                    ],
                    itemStyle:
                    {
                        color: "#55ffdf"
                    }
                }
            ],
        };

        if (option && typeof option === 'object') {
            myChart.setOption(option);
        }

        window.addEventListener('resize', myChart.resize);