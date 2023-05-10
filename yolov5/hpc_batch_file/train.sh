#!/bin/sh
### General options
### –- specify queue --
#BSUB -q gpuv100
### -- set the job Name --
#BSUB -J train
### -- ask for number of cores (default: 1) --
#BSUB -n 8
### -- Select the resources: 1 gpu in exclusive process mode --
#BSUB -gpu "num=2:mode=exclusive_process"
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
#BSUB -o zoom-in-gray-2023-01-30.out
#BSUB -e zoom-in-gray-2023-01-30.err
# -- end of LSF options --

# Load the cuda module
module load cuda/11.5
module load python3/3.8.11
python3 ../train.py --data zoomin-gray.yaml --cfg yolov5s.yaml --name 'zoom-in-gray-2023-01-30'