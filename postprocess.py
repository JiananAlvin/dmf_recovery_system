import math

TL_X = 145
TL_Y = 112
EL_WIDTH = 15.6
EL_HEIGHT = 16
NUM_OF_ELS_IN_A_ROW = 32
NUM_OF_ELS_IN_A_CLMN = 20


# This function maps detection results to electrode ids
def postprocess(arr):
    results = []
    for xyxy in arr:
        min_elid = coor2elid(xyxy[0], xyxy[1])
        max_elid = coor2elid(xyxy[2], xyxy[3])
        results.append(get_ids(min_elid, max_elid))
    return results


# This function gets electrode ids in bounded area
def get_ids(min_elid, max_elid):
    ids = []
    q, r = divmod(max_elid - min_elid, NUM_OF_ELS_IN_A_ROW)
    for i in range(q + 1):
        for j in range(r + 1):
            ids.append(min_elid + i * NUM_OF_ELS_IN_A_ROW + j)
    return ids


# This function maps coordinates (x, y) to an electrode id
def coor2elid(x, y):
    if x < TL_X or y < TL_Y:
        return -1

    clmn = math.floor((x - TL_X) / EL_WIDTH)
    row = math.floor((y - TL_Y) / EL_HEIGHT)

    return row * NUM_OF_ELS_IN_A_ROW + clmn + 1


def main():
    print(coor2elid(165.7, 128))  # 34
    print(coor2elid(145, 112))  # -1
    print(coor2elid(371, 136))  # 47


if __name__ == "__main__":
    main()
