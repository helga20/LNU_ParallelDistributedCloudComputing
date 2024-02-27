set(CMAKE_C_COMPILER "clang")
set(CMAKE_CXX_COMPILER "/usr/bin/clang++")

cmake_minimum_required(VERSION 3.22)

set(CMAKE_CUDA_COMPILER /usr/local/cuda/bin/nvcc)
project(lab_12_1 CUDA)

set(CMAKE_CUDA_STANDARD 23)
add_executable(lab_12_1 main.cu)

set_target_properties(lab_12_1 PROPERTIES
        CUDA_SEPARABLE_COMPILATION ON)
