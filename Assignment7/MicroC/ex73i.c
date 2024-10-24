
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

    for (i = 0; i < n; i=i+1)
        *sump = *sump + arr[i];
}