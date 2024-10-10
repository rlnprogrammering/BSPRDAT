void main(int n) { 
    int ns[7];
    ns[0]=1;
    ns[1]=2;
    ns[2]=1;
    ns[3]=1;
    ns[4]=1;
    ns[5]=2;
    ns[6]=0;

    int i;
    i = 0;
    int freq[4];
    while (i < 4) {
        freq[i]=0;
        i=i+1;
    }
    histogram(7, ns, 3, freq);
    printarr(4, freq);
    println;
}

void histogram(int n, int ns[], int max, int freq[]) {
    int i;
    i = 0;

    while (i < n) {
        int num;
        num = ns[i];
        freq[num] = freq[num] + 1;
        i = i + 1;
    }
}

void printarr(int len, int a[]) {
  int i; 
  i = 0; 
  while (i < len) { 
    print a[i]; 
    i=i+1; 
  } 
}