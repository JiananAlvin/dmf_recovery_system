from pupil_apriltags import Detector
import cv2
import numpy as np

image_path = './image_dataset/biochip.jpg'
img = cv2.imread(image_path, cv2.IMREAD_GRAYSCALE)

at_detector = Detector(
    families="tagStandard41h12",
    nthreads=1,
    quad_decimate=1.0,
    quad_sigma=0.0,
    refine_edges=1,
    decode_sharpening=0.25,
    debug=0
)

results = at_detector.detect(img)

# Specify the four corners of our interested area
# We crop out bleeds of Apriltags by subtracting/adding 25 or 35px
tr = np.array([results[0].corners[2][0] + 25, results[0].corners[2][1] + 35])  # 0 // tr
br = np.array([results[1].corners[3][0] + 25, results[1].corners[3][1] - 35])  # 1
tl = np.array([results[2].corners[1][0] - 25, results[2].corners[1][1] + 35])  # 2
bl = np.array([results[3].corners[0][0] - 25, results[3].corners[0][1] - 35])  # 3 // bl

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

# Save the de-warped image
cv2.imwrite('./image_dataset/warp.png', warp)

# Show the de-warped image
cv2.imshow("warp", warp)
cv2.waitKey(0)
