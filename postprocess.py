import math

TL_X = 145
TL_Y = 112
EL_WIDTH = 15.6
EL_HEIGHT = 16
NUM_OF_ELS_IN_A_ROW = 32
NUM_OF_ELS_IN_A_CLMN = 20
E_WIDTH = 671
E_HEIGHT = 320


# This function maps detection results to electrode ids
def postprocess(arr):
    output = {'e_dimension': [E_WIDTH, E_HEIGHT]}  # [width, height] of the electrodes covered area in px
    d_info = []
    for xyxy in arr:
        print(xyxy)
        # x, y coords of the droplet center & width, height of the droplet in px
        d_info.append([(xyxy[2] + xyxy[0]) / 2, (xyxy[3] + xyxy[1]) / 2, xyxy[2] - xyxy[0], xyxy[3] - xyxy[1]])
    output['d_info'] = d_info
    return output
