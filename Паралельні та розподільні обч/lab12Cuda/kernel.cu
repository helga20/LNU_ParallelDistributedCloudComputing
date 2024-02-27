
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>

//cudaError_t addWithCuda(int *c, const int *a, const int *b, unsigned int size);

__global__ void update(float* life, int* numParticles)
{
    int idx = threadIdx.x + blockIdx.x * blockDim.x;
    if (life[idx] > 0) {
        (*numParticles)++;
    }
}

int main()
{
    int numParticles = 0;
    int* numParticles_d = 0;
    cudaMalloc((void**)&numParticles_d, sizeof(int));
    update << <MAX_PARTICLES / THREADS_PER_BLOCK, THREADS_PER_BLOCK >> > (life, numParticles_d);
    cudaMemcpy(&numParticles, numParticles_d, sizeof(int), cudaMemcpyDeviceToHost);
    printf("%s %i", "numParticles: ", numParticles);

    return 0;
}


