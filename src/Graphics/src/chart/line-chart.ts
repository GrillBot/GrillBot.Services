import { ChartOptions, Label, RequestData } from "./request";
import * as chartJsNodeCanvas from 'chartjs-node-canvas';
import { ChartConfiguration } from "chart.js";

export const renderLineChart = (request: RequestData): Promise<Buffer> => {
    const data = {
        datasets: request.data.datasets.map(o => ({
            label: o.label,
            data: o.data.map(o => ({x: o.label, y: o.value})),
            borderColor: o.color ?? 'black',
            borderWidth: o.width ?? 1
        }))
    };

    const config = createConfig(data, request.options);
    setLabel(config, request.data.topLabel);

    const service = new chartJsNodeCanvas.ChartJSNodeCanvas({
        height: request.options.height,
        width: request.options.width,
        backgroundColour: request.options.backgroundColor
    });

    return service.renderToBuffer(config);
};

const createConfig = (data: any, options: ChartOptions): ChartConfiguration => {
    const config: ChartConfiguration = {
        type: 'line',
        data: data,
        options: {
            responsive: true,
            plugins: { legend: {} },
            elements: { point: {} }
        }
    };

    if (options.legendPosition) { config.options!.plugins!.legend!.position = options.legendPosition; }
    if (options.pointRadius) { config.options!.elements!.point!.radius = options.pointRadius; }

    return config;
};

const setLabel = (config: ChartConfiguration, label?: Label) => {
    if (!label) { return; }

    config!.options!.plugins!.title = {
        display: true,
        text: label.text,
        align: label.align,
        fullSize: true,
        font: {
            size: label.size,
            weight: label.weight
        },
        color: label.color
    };
}
