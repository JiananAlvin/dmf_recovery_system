import publisher
import csv
from datetime import datetime

# TL_X = 145
# TL_Y = 112
# EL_WIDTH = 15.6
# EL_HEIGHT = 16
# NUM_OF_ELS_IN_A_ROW = 32
# NUM_OF_ELS_IN_A_CLMN = 20
IMG_WIDTH = 870
IMG_HEIGHT = 404
TOPIC = "yolo/act"


# This function returns digital info about bounding boxes of detected droplets. The format is
# {'img_dimension': [width, height],
#  'droplet_info': [
#      [x_top_left1, y_top_left1, width1, height1],
#      [x_top_left2, y_top_left2, width2, height2],
#      ...
#  ]
# }
def postprocess(arr):
    output = {'img_dimension': [IMG_WIDTH, IMG_HEIGHT]}  # [width, height] of the electrodes covered area in px
    d_info = []
    for xyxy in arr:
        print(xyxy)
        # the x and y coordinates of the top-left corner of the current bounding box, as well as its width and height
        # all in px
        d_info.append([xyxy[0], xyxy[1], xyxy[2] - xyxy[0], xyxy[3] - xyxy[1]])
    output['droplet_info'] = d_info
    # return output

    publisher.publisher(TOPIC, str(output))

    # For performance test
    # Step 1: Read the contents of the CSV file
    with open('./detection_part_perf.csv', 'r', newline='') as f:
        reader = csv.reader(f)
        rows = [row for row in reader]
        f.close()
    print("================")
    print(rows)

    # Step 2: Append the finish to the last row
    if rows:  
        rows[-1].append(datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f"))
    print("================AA")
    print(rows)

    # Step 3: Write the modified contents back to the file
    with open('your_file.csv', 'w', newline='') as f:
        writer = csv.writer(f)
        writer.writerows(rows)
        f.close()
