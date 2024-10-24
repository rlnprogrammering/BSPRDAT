
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
    i = 0;

    while (i < n) {
        arr[i] = i*i;
        i=i+1; 
    }
}

void arrsum(int n, int arr[], int *sump) {
    int i;
    i = 0;

    while (i < n) {
        *sump = *sump + arr[i];
        i=i+1; 
    }
}