import { Image, Canvas, CanvasRenderingContext2D, loadImage } from "canvas";
import { CanvasHelper } from "../../../common/helper";
import { PeepoImageBase, readTemplate } from "./peepo-base";

const peepoAngryImage = readTemplate('peepoangry.png');

export class PeepoAngry extends PeepoImageBase {
    constructor(avatarFrames: string[]) {
        super(avatarFrames, 250, 105);
    }

    protected async renderConcreteFrame(profilePictureFrame: Image, context: CanvasRenderingContext2D): Promise<void> {
        const circleAvatar = await CanvasHelper.createCircleImage(profilePictureFrame);

        context.drawImage(circleAvatar, 20, 10);
        context.drawImage(await loadImage(peepoAngryImage), 115, -5);
    }
}
