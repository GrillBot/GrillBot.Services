import { Image, Canvas, CanvasRenderingContext2D, loadImage } from 'canvas';
import { CanvasHelper } from '../../../common/helper';
import { PeepoImageBase, readTemplate } from './peepo-base';

const peepoBody = readTemplate('body.png');
const peepoHands = readTemplate('hands.png');
const topOffset = -115;

export class PeepoLove extends PeepoImageBase {
    constructor(avatarFrames: string[]) {
        super(avatarFrames, 512, 397);
    }

    protected async renderConcreteFrame(profilePictureFrame: Image, context: CanvasRenderingContext2D): Promise<void> {
        const circleAvatar = await CanvasHelper.createCircleImage(profilePictureFrame);
        
        context.drawImage(await loadImage(peepoBody), 0, topOffset);
        context.drawImage(circleAvatar, 5, 200, 180, 180);
        context.drawImage(await loadImage(peepoHands), 0, topOffset);
    }
}
