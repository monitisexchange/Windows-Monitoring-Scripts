.386
.model flat, c

.data
	EnterFunction dd 0
	LeaveFunction dd 0
	TailFunction  dd 0
.code

NakedProc MACRO  function
	mov     eax,[ebp+14h]
	push    eax
	mov     ecx,[ebp+10h]
	push    ecx
	mov     edx,[ebp+0Ch]
	push    edx
	mov     eax,[ebp+8h]
	push    eax
	call    function
ENDM

EnterNakedProc proc
	push    ebp
	mov     ebp,esp
	pushad
	cmp EnterFunction, 0
	je EnterNakedEnd

	NakedProc EnterFunction 
EnterNakedEnd:
	popad
	pop     ebp
	ret     16
EnterNakedProc endp

LeaveNakedProc proc
	push    ebp
	mov     ebp,esp
	pushad
	cmp EnterFunction, 0
	je LeaveNakedEnd

	NakedProc LeaveFunction
LeaveNakedEnd:
        popad
        pop     ebp
        ret     16
LeaveNakedProc endp

TailNakedProc proc
	push    ebp
	mov     ebp,esp
	pushad
	cmp TailFunction, 0
	je TailNakedEnd
	
	mov     ecx,[ebp+10h]
	push    ecx
	mov     edx,[ebp+0Ch]
	push    edx
	mov     eax,[ebp+8h]
	push    eax
	call    TailFunction  
	
TailNakedEnd:
	popad
	pop     ebp
	ret     12
TailNakedProc endp

SetHandleProcs proc
	push    ebp                 
	mov     ebp,esp
	;int 3

	mov     eax, [ebp+16]
	mov		TailFunction, eax
	mov     eax, [ebp+12]
	mov		LeaveFunction, eax
	mov     eax, [ebp+8]
	mov		EnterFunction, eax
	pop     ebp
	ret
SetHandleProcs endp
end