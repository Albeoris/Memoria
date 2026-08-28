; QueueUserAPC exposes only three arguments. Unity 5.2 Mono's notify_thread keeps
; the associated DebuggerTlsData and MonoInternalThread pointers in RBX and RDI.
; Forward those values and the caller's return address to the typed C++ handler.

option casemap:none

EXTERN QueueUserApcHookImpl:PROC

.code

QueueUserApcHook PROC FRAME
    sub rsp, 38h
    .allocstack 38h
    .endprolog
    mov [rsp+20h], rdi
    mov rax, [rsp+38h]
    mov [rsp+28h], rax
    mov r9, rbx
    call QueueUserApcHookImpl
    add rsp, 38h
    ret
QueueUserApcHook ENDP

END
