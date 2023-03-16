import { LayoutPosition } from "chart.js";

export interface ChartOptions {
    width: number;
    height: number;
    backgroundColor: string;
    type: 'line';
    legendPosition?: LayoutPosition,
    pointRadius?: number
}

export interface Label {
    text: string;
    align: 'start' | 'end' | 'center';
    size: number;
    color: string;
    weight: string;
}

export interface DataPoint {
    label: string;
    value?: number;
}

export interface Dataset {
    label: string;
    data: DataPoint[];
    color?: string;
    width?: number;
}
export interface ChartData {
    topLabel?: Label;
    datasets: Dataset[];
}

export interface RequestData {
    options: ChartOptions;
    data: ChartData;
}
