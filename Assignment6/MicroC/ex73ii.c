
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

    for (i = 0; i < n; i=i+i)
        arr[i] = i*i;
}

void arrsum(int n, int arr[], int *sump) {

    for (int i = 0; i < n; i=i+i)
        *sump = *sump + arr[i];
}