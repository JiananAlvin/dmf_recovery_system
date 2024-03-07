from pupil_apriltags import Detector
import cv2
import numpy as np
import csv 
from datetime import datetime

BLEED_X = 42
BLEED_Y = 188
WIDTH = 954
HEIGHT = 780
# image_path = './image_dataset/biochip.jpg'
# img = cv2.imread(image_path, cv2.IMREAD_GRAYSCALE)


def preprocess(img):
    # For performance test
    # Open or create the CSV file to append the start time
    with open('./detection_part_perf_1.10.csv', 'a', newline='') as f:
        writer = csv.writer(f)
        writer.writerow([datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f")])
        f.close()

    at_detector = Detector(
        families="tagStandard41h12",
        nthreads=1,
        quad_decimate=1.0,
        quad_sigma=0.0,
        refine_edges=1,
        decode_sharpening=0.25,
        debug=0
    )

    # RGB to Grayscale
    gray_img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    results = at_detector.detect(gray_img)

    # Specify the four corners of our interested area
    tr = np.array([results[0].corners[2][0], results[0].corners[2][1]])  # 0 // tr
    br = np.array([results[1].corners[3][0], results[1].corners[3][1]])  # 1
    tl = np.array([results[2].corners[1][0], results[2].corners[1][1]])  # 2
    bl = np.array([results[3].corners[0][0], results[3].corners[0][1]])  # 3 // bl

    # Now that we have our rectangle of points, let's compute
    # The width of our new image
    rect = np.array([tl, tr, br, bl], dtype="float32")
    width_a = np.sqrt(((br[0] - bl[0]) ** 2) + ((br[1] - bl[1]) ** 2))
    width_b = np.sqrt(((tr[0] - tl[0]) ** 2) + ((tr[1] - tl[1]) ** 2))

    # And now for the height of our new image
    height_a = np.sqrt(((tr[0] - br[0]) ** 2) + ((tr[1] - br[1]) ** 2))
    height_b = np.sqrt(((tl[0] - bl[0]) ** 2) + ((tl[1] - bl[1]) ** 2))

    # Take the maximum of the width and height values to reach
    # Our final dimensions
    max_width = max(int(width_a), int(width_b))
    max_height = max(int(height_a), int(height_b))

    # Construct our destination points which will be used to
    # Map the screen to a top-down, "birds eye" view
    dst = np.array([
        [0, 0],
        [max_width - 1, 0],
        [max_width - 1, max_height - 1],
        [0, max_height - 1]], dtype="float32")

    # Calculate the perspective transform matrix and warp
    # The perspective to grab the screen
    M = cv2.getPerspectiveTransform(rect, dst)
    warp = cv2.warpPerspective(img, M, (max_width, max_height))
    # cv2.imwrite('../../test_video/warp{}.png'.format(0), warp)

    # Always resize image to a specific dimension to make sure we can crop out the active biochip area by subtracting
    # the same bleeds
    warp = cv2.resize(warp, dsize=(WIDTH, HEIGHT), interpolation=cv2.INTER_CUBIC)
    # cv2.imwrite('../../test_video/resize{}.png'.format(0), warp)

    # Crop the image to get the active biochip area, warp.shape[0] == height, warp.shape[1] == width
    active_area = warp[BLEED_Y:warp.shape[0]-BLEED_Y, BLEED_X:warp.shape[1]-BLEED_X, :]
    # cv2.imwrite('../../test_video/active_area{}.png'.format(0), active_area)

    # Return the de-warped image
    return active_area

# # Save the de-warped image
# cv2.imwrite('./image_dataset/warp.png', warp)
#
# # Show the de-warped image
# cv2.imshow("warp", warp)
# cv2.waitKey(0)
