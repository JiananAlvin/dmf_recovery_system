#!/bin/sh
### General options
### –- specify queue --
#BSUB -q gpuv100
### -- set the job Name --
#BSUB -J val
### -- ask for number of cores (default: 1) --
#BSUB -n 8
### -- Select the resources: 1 gpu in exclusive process mode --
#BSUB -gpu "num=1:mode=exclusive_process"
### -- set walltime limit: hh:mm --  maximum 24 hours for GPU-queues right now
#BSUB -W 24:00
# request 5GB of system-memory
#BSUB -R "rusage[mem=10GB]"
### -- set the email address --
# please uncomment the following line and put in your e-mail address,
# if you want to receive e-mail notifications on a non-default address
##BSUB -u s204698@student.dtu.dk
### -- send notification at start --
#BSUB -B
### -- send notification at completion--
#BSUB -N
### -- Specify the output and error file. %J is the job-id --
### -- -o and -e mean append, -oo and -eo mean overwrite --
#BSUB -o gpu-val-2023-01-16.out
#BSUB -e gpu-val-2023-01-16.err
# -- end of LSF options --

# Load the cuda module
module load cuda/11.5
module load python3/3.8.11
python3 ../val.py --data zoomin-gray.yaml --weights '../runs/train/zoom-in-gray/weights/best.pt' --name 'zoomin-gray'
python3 ../val.py --data zoomin-rgb.yaml --weights '../runs/train/zoom-in-rgb/weights/best.pt' --name 'zoomin-rgb'
python3 ../val.py --data full-gray-without-hand.yaml --weights '../runs/train/full-gray-without-hand/weights/best.pt' --name 'full-gray-without-hand'
python3 ../val.py --data full-rgb-without-hand.yaml --weights '../runs/train/full-rgb-without-hand/weights/best.pt' --name 'full-rgb-without-hand'
python3 ../val.py --data full-gray-with-hand.yaml --weights '../runs/train/full-gray-with-hand/weights/best.pt' --name 'full-gray-with-hand'
python3 ../val.py --data full-rgb-with-hand.yaml --weights '../runs/train/full-rgb-with-hand/weights/best.pt' --name 'full-rgb-with-hand'
python3 ../val.py --data full-gray-bubble.yaml --weights '../runs/train/new-model-integrated-gray/weights/best.pt' --name 'new_model_intergrated_gray'
python3 ../val.py --data full-rgb-bubble.yaml --weights '../runs/train/new-model-integrated-rgb/weights/best.pt' --name 'new_model_intergrated_rgb'