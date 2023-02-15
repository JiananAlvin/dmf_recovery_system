import math

TL_X = 145
TL_Y = 112
EL_WIDTH = 15.6
EL_HEIGHT = 16
NUM_OF_ELS_IN_A_ROW = 32
NUM_OF_ELS_IN_A_CLMN = 20


# This function maps bounded area to electrode id
def postprocess(x, y):
    if x < TL_X or y < TL_Y:
        return -1

    clmn = math.floor((x - TL_X) / EL_WIDTH)
    row = math.floor((y - TL_Y) / EL_HEIGHT)

    return row * NUM_OF_ELS_IN_A_ROW + clmn + 1


def main():
    print(postprocess(165.7, 128))  # 34
    print(postprocess(145, 112))  # -1
    print(postprocess(371, 136))  # 47


if __name__ == "__main__":
    main()
