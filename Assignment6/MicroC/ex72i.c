
void main(int n) { 
    int a[4];
    int sum;
    sum = 0;
    a[0]=7;
    a[1]=13;
    a[2]=9;
    a[3]=8;
    arrsum(n, a, &sum);
    print sum;
    println;
}

void arrsum(int n, int arr[], int *sump) {
    int i;
    i = 0;

    while (i < n) {
        *sump = *sump + arr[i];
        i=i+1; 
    }
}