#include <stdio.h>
#include <gigatron/sys.h>
#include <gigatron/console.h>
#include <gigatron/libc.h>

struct MusDat
{
    int adr;
	byte dat;
};

#include "gigatronTest.h"

void init_channel(){
    channel1.keyL=0;
    channel2.keyL=0;
    channel3.keyL=0;
    channel4.keyL=0;
    
    channel1.keyH=0;
    channel2.keyH=0;
    channel3.keyH=0;
    channel4.keyH=0;
    
}


int main()
{
  register struct MusDat *md = mus;
  register int tmpfc = frameCount;
  register int wait=0;
  init_channel();

  printf("Gigatron music engine\n");
  
  while(md->adr != 0){

    if(md->adr == 1){
        wait = md->dat;
        while(wait > 0){
            while (frameCount == tmpfc){ /* wait */ }
            tmpfc = frameCount;
            soundTimer = 1;
            wait--;
        }
    }
    else if(md->adr == 2){
    	md = mus;//loop
    	continue;
    }
    else{
         (*(byte*)(md->adr)) = (byte)md->dat;
    }

    md++;

  }
    
  printf("Completion of the song\n");
  return 0;
}
