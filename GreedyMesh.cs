//NOTE: this code was ported to C#, original: https://github.com/mikolalysenko/mikolalysenko.github.com/blob/gh-pages/MinecraftMeshes/js/greedy.js

using System;
using System.Collections.Generic;

public static class GreedyMesh
{
   private const int SIZE = 1<<4;

   public static List<float[][]> Generate(int[][][] data)
   {
      Func<int, int, int, bool> f = (i, j, k) =>
      {
         return data[i][j][k] != 0;
      };
      
      List<float[][]> quads = new List<float[][]>();
      int[] dimensions = {SIZE, SIZE, SIZE};
      
      //Sweep over 3-axes
      for(var d=0; d<3; ++d) {
         int i, j, k, l, w, h, u = (d+1)%3, v = (d+2)%3;
         int[] x = new int[3];
         int[] q = new int[3];

         bool[] mask = new bool[dimensions[u] * dimensions[v]];

         q[d] = 1;
         for(x[d]=-1; x[d]<dimensions[d]; ) {
            //Compute mask
            var n = 0;
            for(x[v]=0; x[v]<dimensions[v]; ++x[v])
            for(x[u]=0; x[u]<dimensions[u]; ++x[u]) {
               mask[n++] =
                  (0 <= x[d] && f(x[0], x[1], x[2])) !=
                  (x[d] < dimensions[d] - 1 && f(x[0] + q[0], x[1] + q[1], x[2] + q[2]));
               }
            //Increment x[d]
            ++x[d];
            //Generate mesh for mask using lexicographic ordering
            n = 0;
            for(j=0; j<dimensions[v]; ++j)
            for(i=0; i<dimensions[u]; ) {
               if(mask[n]) {
                  //Compute width
                  for(w=1; mask[n+w] && i+w<dimensions[u]; ++w) {
                  }
                  //Compute height (this is slightly awkward
                  var done = false;
                  for(h=1; j+h<dimensions[v]; ++h) {
                     for(k=0; k<w; ++k) {
                        if(!mask[n+k+h*dimensions[u]]) {
                           done = true;
                           break;
                        }
                     }
                     if(done) {
                        break;
                     }
                  }
                  //Add quad
                  x[u] = i;  x[v] = j;
                  int[] du = new int[3]; du[u] = w;
                  int[] dv = new int[3]; dv[v] = h;
                  quads.Add(new[]
                  {
                     new float[] {x[0], x[1], x[2]},
                     new float[] {x[0]+du[0], x[1]+du[1], x[2]+du[2]},
                     new float[] {x[0]+du[0]+dv[0], x[1]+du[1]+dv[1], x[2]+du[2]+dv[2]},
                     new float[] {x[0]+dv[0], x[1]+dv[1], x[2]+dv[2]}
                  });
                  //Zero-out mask
                  for(l=0; l<h; ++l)
                  for(k=0; k<w; ++k) {
                     mask[n+k+l*dimensions[u]] = false;
                  }
                  //Increment counters and continue
                  i += w; n += w;
               } else {
                  ++i;    ++n;
               }
            }
         }
      }
      return quads;
   }
}
