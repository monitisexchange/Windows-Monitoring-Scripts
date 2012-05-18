.data
	EnterFunction dq 0
	LeaveFunction dq 0
	TailFunction  dq 0
.code

pushaq MACRO  function
	push	rax
	push	rcx
	push	rdx
	push	rbx
	;push	rsp
	push	rbp
	push	rsi
	push	rdi
	push	r8
	push	r9
	push	r10
	push	r11
	push	r12
	push	r13
	push	r14
	push	r15
endm

popaq MACRO  function
	pop	r15
	pop	r14
	pop	r13
	pop	r12
	pop	r11
	pop	r10
	pop	r9
	pop	r8
	pop	rdi
	pop	rsi
	pop	rbp
	;pop	rsp
	pop	rbx
	pop	rdx
	pop	rcx
	pop	rax
endm

EnterNakedProc proc
	;int	3
	pushfq
	pushaq
	cmp EnterFunction, 0
	je EnterNakedEnd

	call EnterFunction
EnterNakedEnd:
	popaq
	popfq
	;int 3
	ret
EnterNakedProc endp

LeaveNakedProc proc
	;int 3
	pushfq
	pushaq
	
	cmp LeaveFunction, 0
	je LeaveNakedEnd

	call LeaveFunction
LeaveNakedEnd:
	popaq
	popfq
	ret
LeaveNakedProc endp

TailNakedProc proc
	;int	3
	pushfq
	pushaq
	
	cmp TailFunction, 0
	je TailNakedEnd
	
	call    TailFunction  
TailNakedEnd:
	popaq
	popfq
	ret
TailNakedProc endp

SetHandleProcs proc
	push    rbp                 
	mov     rbp,rsp

	mov	TailFunction, r8
	mov	LeaveFunction, rdx
	mov	EnterFunction, rcx
	pop     rbp
	ret
SetHandleProcs endp
end