/*
 * =====================================================
 *  CONCURRENCY CHALLENGE: Multithreaded Bank System
 * =====================================================
 *
 * This program simulates a bank with multiple accounts.
 * Several threads perform deposits, withdrawals, and
 * transfers concurrently.
 *
 * YOUR MISSION:
 *   There is ONE deliberate concurrency bug hidden in
 *   this program. Find it, understand why it's a bug,
 *   and fix it.
 *
 * CONCEPTS COVERED:
 *   - pthreads (POSIX threads)
 *   - Mutexes (pthread_mutex_t)
 *   - Race conditions
 *   - Deadlock
 *   - Atomic operations
 *
 * COMPILE:
 *   gcc -Wall -g -o bank bank_threads.c -lpthread
 *
 * RUN:
 *   ./bank
 *
 * HINT (read only if stuck):
 *   The bug is in the transfer() function.
 *   Think about what happens when two threads try to
 *   transfer money between the same two accounts at
 *   the same time, but in opposite directions.
 *   Thread A: transfer(account[0], account[1], 100)
 *   Thread B: transfer(account[1], account[0], 50)
 *   Draw out the lock acquisition order...
 * =====================================================
 */

#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <unistd.h>

#define NUM_ACCOUNTS   4
#define NUM_THREADS    8
#define NUM_OPS        500

/* ── Account structure ─────────────────────────────── */
typedef struct {
    int             id;
    int             balance;
    pthread_mutex_t lock;
} Account;

/* ── Globals ───────────────────────────────────────── */
Account accounts[NUM_ACCOUNTS];

/* ── Helpers ───────────────────────────────────────── */
void account_init(Account *a, int id, int balance) {
    a->id      = id;
    a->balance = balance;
    pthread_mutex_init(&a->lock, NULL);
}

void deposit(Account *a, int amount) {
    pthread_mutex_lock(&a->lock);
    a->balance += amount;
    pthread_mutex_unlock(&a->lock);
}

int withdraw(Account *a, int amount) {
    pthread_mutex_lock(&a->lock);
    if (a->balance < amount) {
        pthread_mutex_unlock(&a->lock);
        return -1;   /* insufficient funds */
    }
    a->balance -= amount;
    pthread_mutex_unlock(&a->lock);
    return 0;
}

/*
 * transfer() — moves `amount` from `src` to `dst`.
 *
 * BUG IS HERE.
 * The locks are acquired in the order: src first, then dst.
 * This is fine *unless* another thread does the reverse
 * (dst first, then src) at the same moment — classic deadlock.
 *
 * Fix: always acquire locks in a consistent global order,
 * e.g. by account id:
 *
 *   Account *first  = (src->id < dst->id) ? src : dst;
 *   Account *second = (src->id < dst->id) ? dst : src;
 *   pthread_mutex_lock(&first->lock);
 *   pthread_mutex_lock(&second->lock);
 *   ... do the transfer ...
 *   pthread_mutex_unlock(&second->lock);
 *   pthread_mutex_unlock(&first->lock);
 */
int transfer(Account *src, Account *dst, int amount) {
    /* BUG: lock order depends on caller — can deadlock */
    pthread_mutex_lock(&src->lock);

    /* Simulate a tiny delay — makes the bug easier to trigger */
    usleep(1);

    pthread_mutex_lock(&dst->lock);   /* <-- deadlock risk */

    if (src->balance < amount) {
        pthread_mutex_unlock(&dst->lock);
        pthread_mutex_unlock(&src->lock);
        return -1;
    }
    src->balance -= amount;
    dst->balance += amount;

    pthread_mutex_unlock(&dst->lock);
    pthread_mutex_unlock(&src->lock);
    return 0;
}

/* ── Thread worker ─────────────────────────────────── */
void *worker(void *arg) {
    int tid = *(int *)arg;

    for (int i = 0; i < NUM_OPS; i++) {
        int op  = rand() % 3;
        int src = rand() % NUM_ACCOUNTS;
        int dst = rand() % NUM_ACCOUNTS;
        int amt = (rand() % 50) + 1;

        if (op == 0) {
            deposit(&accounts[src], amt);
        } else if (op == 1) {
            withdraw(&accounts[src], amt);
        } else {
            if (src != dst)
                transfer(&accounts[src], &accounts[dst], amt);
        }
    }

    printf("Thread %d finished.\n", tid);
    return NULL;
}

/* ── Audit: total balance should be constant ───────── */
int total_balance(void) {
    int total = 0;
    for (int i = 0; i < NUM_ACCOUNTS; i++) {
        pthread_mutex_lock(&accounts[i].lock);
        total += accounts[i].balance;
        pthread_mutex_unlock(&accounts[i].lock);
    }
    return total;
}

/* ── Main ──────────────────────────────────────────── */
int main(void) {
    srand(42);

    /* Initialise accounts with $1000 each */
    int initial_total = 0;
    for (int i = 0; i < NUM_ACCOUNTS; i++) {
        account_init(&accounts[i], i, 1000);
        initial_total += 1000;
    }

    printf("=== Multithreaded Bank ===\n");
    printf("Initial total balance: $%d\n\n", initial_total);
    printf("Spawning %d threads, each doing %d operations...\n\n",
           NUM_THREADS, NUM_OPS);

    pthread_t threads[NUM_THREADS];
    int       tids[NUM_THREADS];

    for (int i = 0; i < NUM_THREADS; i++) {
        tids[i] = i;
        pthread_create(&threads[i], NULL, worker, &tids[i]);
    }

    for (int i = 0; i < NUM_THREADS; i++) {
        pthread_join(threads[i], NULL);
    }

    printf("\nAll threads done.\n");

    int final = total_balance();
    printf("Final total balance : $%d\n", final);
    printf("Expected            : $%d\n", initial_total);

    if (final == initial_total) {
        printf("Balance check PASSED (transfers preserved money)\n");
    } else {
        printf("Balance check FAILED — money was created or destroyed!\n");
    }

    /* Cleanup */
    for (int i = 0; i < NUM_ACCOUNTS; i++)
        pthread_mutex_destroy(&accounts[i].lock);

    return 0;
}
