.DEFAULT_GOAL := hw

.PHONY: hw lab prac

hw lab prac:
	$(MAKE) -C $@ run
