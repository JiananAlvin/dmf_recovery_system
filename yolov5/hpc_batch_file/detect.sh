#!/bin/sh
### General options
### –- specify queue --
#BSUB -q gpuv100
### -- set the job Name --
#BSUB -J detect
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
#BSUB -o gpu-detect-new-model-integrated-rgb-2023-01-30.out
#BSUB -e gpu-detect-new-model-integrated-rgb-2023-01-30.err
# -- end of LSF options --

# Load the cuda module
module load cuda/11.5
module load python3/3.8.11
python3 ../detect.py --weights '../runs/train/new-model-integrated-rgb/weights/best.pt' --source '../../image_dataset/dataset-droplet-2022-8-19-full-rgb-bubble/test/images/1652453576-040376_jpg.rf.fbb5e10d332e7b04cf2ad55b2cbe30e0.jpg' --name 'gpu-detect-new-model-integrated-rgb-2023-01-30'