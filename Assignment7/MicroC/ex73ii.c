
void main(int n) { 
    int sum;
    sum = 0;
    if (n < 21) {
        int a[20];
        squares(n, a);
        arrsum(n, a, &sum);
        print sum;
    }
    println;
}

void squares(int n, int arr[]) {
    int i;

    for (i = 0; i < n; i=i+1) 
        arr[i] = i*i;
}

void arrsum(int n, int arr[], int *sump) {
    int i;
    for (i = 0; i < n; i=i+1)
        *sump = *sump + arr[i];
}