# COSMOS-BASED-CUSTOM-OS
A custom OS built with Cosmos User Kit featuring filesystem, memory, swap and IPC managers, plus a secure system call handler. Coordinated kernel modules handle scheduling, hardware interaction and core services, showcasing low-level systems engineering.

# Overview

This project is a custom, experimental operating system built atop the Cosmos User Kit, focusing on kernel-level systems engineering, subsystem design, and controlled hardware interaction. The OS adopts a modular architecture, where each core component—memory, filesystem, IPC, swapping, system calls, and sandboxing—is encapsulated into dedicated managers coordinated by the kernel core. The goal is to demonstrate how a microkernel-leaning design can be implemented within Cosmos’s environment while still providing a unified, extensible runtime.

## Kernel Architecture

At the heart of the system is a stratified kernel composed of orchestration modules responsible for scheduling, interrupt dispatching, early initialization, and hardware abstraction. These kernel layers mediate device interactions, bootstrap the memory layout, and expose safe interfaces to higher subsystems. The kernel includes structured initialization phases that load the memory subsystem, mount base filesystems, configure process tables, and enable user-mode transitions. Emphasis is placed on clean separation between privileged execution contexts and user applications.

## Memory Management

The memory manager implements physical and virtual allocation strategies using a paging-based model. It abstracts block allocation, page tables, heap expansion, and deallocation policies while ensuring predictable behavior even under constrained physical RAM. Virtual address space organization follows a deterministic layout that isolates kernel space from user processes. Internal fragmentation is minimized through a custom allocator tuned for kernel workloads. The subsystem is designed to cooperate tightly with the swap manager, allowing transparent page relocation.

## Swap Manager

The swap manager extends usable memory through disk-backed page swapping. It operates by monitoring the memory manager’s thresholds and offloading inactive pages into a structured swap region. Custom metadata tracks mappings between virtual pages and swap slots. Although optimized for simplicity rather than performance, the implementation demonstrates the mechanism of paging out and page fault–driven retrieval, showcasing low-level memory pressure handling.

## Filesystem Manager

The filesystem manager provides directory traversal, file CRUD operations, metadata handling, and abstraction over block devices. It interfaces with the kernel’s device layer and uses a modular VFS-style architecture to allow multiple backend filesystem drivers. Caching mechanisms reduce repeated disk access, and internal locking ensures safe concurrent operations. This subsystem also supports mounting and unmounting routines executed during system initialization.

## IPC Manager

Inter-process communication is implemented through message queues and structured signaling. The IPC manager maintains per-process buffers, routing logic, and synchronization primitives. It guarantees isolation between processes while enabling controlled data exchange. The subsystem integrates with the scheduler to avoid deadlocks by enforcing non-blocking semantics wherever possible. IPC operations are exposed to user programs through the system call interface.

## System Call Handler

The system call handler forms the primary boundary between user applications and kernel services. Each call is validated for correctness and permission compliance before being executed. Argument passing, privilege verification, and context switching are handled through a carefully isolated interface that preserves kernel integrity. The handler exposes filesystem operations, IPC primitives, memory requests, and process management functions, while sanitizing all user-provided data.

## Sandboxing (Experimental)

A lightweight sandboxing mechanism provides limited process isolation beyond standard privilege separation. It restricts filesystem access patterns, IPC visibility, and system call availability based on per-process policies. Although not a full security sandbox, it demonstrates the foundational concepts: capability-limited execution, namespace partitioning, and rule-based system call filtering. The sandbox framework is extensible, allowing additional constraints or namespace models to be added later.

## Build and Booting

The system is built through the Cosmos User Kit toolchain, which compiles the kernel into a bootable ISO. Booting is supported on emulators such as QEMU, VMware, or VirtualBox. Kernel startup logs provide detailed traces for debugging, including memory map detection, driver initialization, and subsystem registration.

## Goals and Future Extensions

This OS is intended as a research and learning platform rather than a production environment. Future enhancements may include a more complete scheduler, a refined sandboxing model, filesystem journaling, improved swap performance, and a richer set of user-mode utilities.
