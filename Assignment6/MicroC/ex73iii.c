void main() { 
    int ns[7];
    ns[0]=1;
    ns[1]=2;
    ns[2]=1;
    ns[3]=1;
    ns[4]=1;
    ns[5]=2;
    ns[6]=0;

    int freq[4]; // freq length 4, meaning maxlength is 3

    int i;
    for (i = 0;i < 4;i=i+1) {
        freq[i]=0;
    }
    histogram(7, ns, 3, freq);
    printarr(4, freq);
    println;
}

void histogram(int n, int ns[], int max, int freq[]) {
    int i;

    for (i = 0;i < n;i=i+1) {
        int num;
        num = ns[i];
        freq[num] = freq[num] + 1;
    }
}

void printarr(int len, int a[]) {
  int i; 
  for (i = 0;i < len;i=i+1) {
    print a[i]; 
  } 
}