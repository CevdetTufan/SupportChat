# SupportChat

SupportChat is a polling-based support chat backend that manages real-time help requests, agent assignment, and session lifecycle according to predefined business rules.

## Key Principles

1. **Queue and Assignment**

   * Users submit a request via `POST`; a chat session is created and placed in a FIFO queue.
   * A background worker picks the oldest unassigned session and assigns it exactly once to an `Active` agent with available capacity.

2. **Capacity and Overflow**

   * Agents can handle up to 10 concurrent chats, scaled by a seniority multiplier (0.4–0.8) and rounded down.
   * The maximum queue length is 1.5× the team’s total capacity. During office hours, if that limit is reached, an overflow team of junior-level agents is engaged.

3. **Shift Management**

   * Agents work in three 8-hour shifts.
   * At shift end, agents complete ongoing chats but accept no new ones, then transition to `Inactive`.

4. **Polling and Timeouts**

   * After receiving a successful response, the chat window polls `POST /poll` every second.
   * If a session misses three consecutive polls, it is marked `Inactive` and the agent’s load is freed.

5. **Round-Robin Strategy**

   * Chats are distributed in rotation: juniors first, then mid-levels, then seniors/team leads—ensuring fair load distribution.


