# SupportChat

1. **Send a Request**
  `POST` and chat session joins a FIFO queue.
2. **Agent Assignment**
   A background service grabs the oldest waiting session and pairs it with an agent who’s active and has room for one more chat.
3. **Keep-Alives**
   Once you see an “OK,” your chat window checks in every second via `POST /poll`. Miss three in a row, and the session closes automatically.
4. **Agent Shifts & Capacity**
   Agents work three shifts (8 hours each). When a shift ends, they finish their current chats but won’t take new ones. Each agent can handle up to 10 chats, adjusted by their seniority.
5. **Overflow Mode**
   If the queue grows beyond 1.5× the team’s capacity during office hours, junior overflow agents join in to help.
